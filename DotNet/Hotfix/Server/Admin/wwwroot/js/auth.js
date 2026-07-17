window.gdkAdminAuth = {
    async login(username, password) {
        try {
            const response = await fetch("/api/auth/login", {
                method: "POST",
                credentials: "same-origin",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username, password })
            });
            const payload = await response.json();
            return {
                success: response.ok && payload.success === true,
                errorMessage: payload.errorMessage || "登录失败，请稍后重试",
                user: payload.user || null
            };
        } catch {
            return {
                success: false,
                errorMessage: "无法连接管理服务，请检查网络后重试",
                user: null
            };
        }
    },

    async logout() {
        try {
            const response = await fetch("/api/auth/logout", {
                method: "POST",
                credentials: "same-origin"
            });
            return response.ok;
        } catch {
            return false;
        }
    }
};
