using System;

namespace FitnessClub_Test.Dtos
{
    public class NotificationDTO
    {
        public int User_ID { get; set; } 
        public string Type {  get; set; }
        public string Message {  get; set; }
        public DateTime Date {  get; set; } 
        public string DeliveryMethod {  get; set; }
    }
}
