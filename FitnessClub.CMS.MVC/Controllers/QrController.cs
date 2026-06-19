using FitnessClub_Test.CMS.MVC.Services;
using FitnessClub_Test.Core.NewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using QRCoder;
using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace FitnessClub_Test.CMS.MVC.Controllers
{
    [Authorize]
    public class QrController : BaseController
    {
        private readonly ApiAuthClient _api;
        public QrController(ApiAuthClient api) : base(api)
        {
            _api = api;
        }

        public IActionResult Index()
        {
            return View();
        }

        // used for check in out
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateQr()
        {
            var client = await _api.GetAuthorizedClientAsync();
            var response = await client.PostAsync("qr/generate", null);

            if (!response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to generate QR code"
                });
            }

            var json = await response.Content.ReadAsStringAsync();

            var QrToken = JObject.Parse(json)["token"]?.ToString();
            var type = JObject.Parse(json)["type"]?.ToString();

            using var gen = new QRCodeGenerator();
            var data = gen.CreateQrCode(QrToken, QRCodeGenerator.ECCLevel.Q);
            var qrBytes = new PngByteQRCode(data).GetGraphic(20);

            return Json(new
            {
                success = true,
                qrCodeBase64 = $"data:image/png;base64,{Convert.ToBase64String(qrBytes)}",
                QrToken,
                type
            });
        }

        // used for first time user create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateFixedQr()
        {
            var client = await _api.GetAuthorizedClientAsync();
            var response = await client.PostAsync("qr/generate/fixed-temp", null);

            if (!response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to generate QR code"
                });
            }

            var json = await response.Content.ReadAsStringAsync();

            var qrToken = JObject.Parse(json)["token"]?.ToString();

            using var gen = new QRCodeGenerator();
            var data = gen.CreateQrCode(qrToken, QRCodeGenerator.ECCLevel.Q);
            var qrBytes = new PngByteQRCode(data).GetGraphic(20);

            return Json(new
            {
                success = true,
                qrCodeBase64 = $"data:image/png;base64,{Convert.ToBase64String(qrBytes)}",
                qrToken
            });
        }

        // used for updating existing qr
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateUpdated(int UserID)
        {
            var client = await _api.GetAuthorizedClientAsync();
            var response = await client.PostAsync($"qr/generate/updated/{UserID}", null);

            if (!response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to generate QR code"
                });
            }

            var json = await response.Content.ReadAsStringAsync();
            var qrValue = JObject.Parse(json)["qrValue"]?.ToString();

            using var gen = new QRCodeGenerator();
            var data = gen.CreateQrCode(qrValue, QRCodeGenerator.ECCLevel.Q);
            var qrBytes = new PngByteQRCode(data).GetGraphic(20);

            return Json(new
            {
                success = true,
                qrCodeBase64 = $"data:image/png;base64,{Convert.ToBase64String(qrBytes)}",
                qrValue
            });
        }
    }
        
}
