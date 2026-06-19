async function apiFetch(url, options = {}) {

    let token = window.authToken;

    let response = await fetch(url, {
        ...options,
        headers: {
            ...(options.headers || {}),
            Authorization: `Bearer ${token}`
        }
    });

    // TOKEN EXPIRED HANDLING
    if (response.status === 401) {

        // call MVC refresh endpoint
        const refreshRes = await fetch('/UserAuth/Refresh', {
            method: 'POST'
        });

        if (!refreshRes.ok) {
            window.location.href = "/Login/Index";
            return;
        }

        const data = await refreshRes.json();

        // update frontend memory token
        window.authToken = data.token;

        // retry original request
        response = await fetch(url, {
            ...options,
            headers: {
                ...(options.headers || {}),
                Authorization: `Bearer ${window.authToken}`
            }
        });
    }

    return response;
}