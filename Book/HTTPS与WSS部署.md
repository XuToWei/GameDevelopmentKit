# 使用 Nginx 为微信小程序配置 HTTPS 和 WSS

小程序登录时，先通过 HTTPS 获取路由列表，再通过 WSS 连接 ET Router。下面以 `game.example.com` 为例，假设 Nginx 和 ET 服务端在同一台 Linux 服务器上。Nginx 在公网 `443` 端口处理 TLS，再将两个请求分别转发到本机服务。[Nginx 配置模板](../Tools/Config/nginx-router.conf)

| 用途 | 小程序访问地址 | Nginx 转发目标 |
| --- | --- | --- |
| 查询路由列表 | `https://game.example.com/get_router` | RouterManager `http://127.0.0.1:30300/get_router` |
| 建立游戏连接 | `wss://game.example.com/ws/` | Router01 `ws://127.0.0.1:30301/ws/` |

## 1. 准备域名和 ET 服务端

1. 将游戏域名解析到 Nginx 的公网地址，允许客户端访问 TCP `443`。为该域名申请受信任的 TLS 证书，准备好完整证书链和对应私钥。
2. 在 `DotNet.Hotfix.csproj` 实际使用的构建配置中，将 `<DefineConstants>DOTNET</DefineConstants>` 改为 `<DefineConstants>DOTNET;UNITY_WEBGL</DefineConstants>`。重新构建并启动 ET 服务端，使 Router 的外网端口进入 WebSocket 监听模式。[Hotfix 项目](../DotNet/Hotfix/DotNet.Hotfix.csproj) · [独立服务端启动步骤](快速开始.md)
3. 核对启动配置里的监听地址。模板使用 RouterManager 的 `30300` 和 Router01 的 `30301`。如果 ET 位于另一台主机或容器，将模板中两处 `proxy_pass` 分别改为 Nginx 能访问的 ET 监听地址。

## 2. 安装并加载 Nginx 配置

在服务器上从仓库根目录执行以下命令。示例使用由 `systemd` 管理的 Nginx，主配置的 `http` 块需包含 `/etc/nginx/conf.d/*.conf`。

```sh
sudo cp Tools/Config/nginx-router.conf /etc/nginx/conf.d/nginx-router.conf
sudoedit /etc/nginx/conf.d/nginx-router.conf
```

将 `server_name` 改为实际游戏域名，将 `ssl_certificate` 和 `ssl_certificate_key` 分别指向完整证书链和私钥文件。保留 `/ws/` 末尾的斜杠、HTTP/1.1、`Upgrade`、`Connection` 和 `Host` 转发设置，ET 的 `HttpListener` 会按后端地址和路径匹配握手请求。保存后检查并加载配置。

```sh
sudo nginx -t && sudo systemctl reload nginx
```

## 3. 配置小程序和客户端

在微信公众平台对应小程序或小游戏的「开发管理 → 开发设置 → 服务器域名」中登记以下两项。后台登记到域名一级，具体路径由客户端使用。

| 微信后台配置项 | 填写示例 |
| --- | --- |
| `request` 合法域名 | `https://game.example.com` |
| `socket` 合法域名 | `wss://game.example.com` |

在客户端 `ConstValue.cs` 中填写同一域名，并重新构建或发布包含该配置的小程序版本。[ConstValue.cs](../Unity/Assets/Scripts/Game/ET/Code/Hotfix/Share/ConstValue.cs)

```csharp
public const string RouterHttpUrl = "https://game.example.com";
public const string RouterWebSocketUrl = "wss://game.example.com/ws/";
```

`RouterHttpUrl` 填到域名末尾，代码会自动追加 `/get_router` 和查询参数。`RouterWebSocketUrl` 填完整路径 `/ws/`。当前模板把这个 WSS 入口固定转发到 Router01。

## 4. 验收和排错

1. 从能访问公网域名的设备执行 `curl -i https://game.example.com/get_router`。预期返回 `200` 和包含 `Routers`、`Realms` 的 JSON，目标环境中两个列表都应有数据。
2. 在微信开发者工具中启用合法域名和证书校验，预览并在真机登录。查看网络记录中的 `wss://game.example.com/ws/`，握手应返回 `101 Switching Protocols`。随后确认客户端完成 Gate 登录，验证游戏数据也能通过代理收发。

| 现象 | 检查位置 |
| --- | --- |
| 域名或证书校验失败 | 核对微信后台的 `request`、`socket` 两项、域名解析、证书覆盖的域名及完整证书链。 |
| `/get_router` 返回 `404` | 核对 `server_name` 和客户端 `RouterHttpUrl`。重复填写 `/get_router` 或额外的尾斜杠都会改变请求路径。 |
| Nginx 返回 `502` | 在 Nginx 所在主机核对后端地址和端口。查询请求对应 `30300`，WebSocket 对应 `30301`。 |
| 查询成功，WSS 握手返回 `400` 或 `404` | 核对 `/ws/` 路径、Upgrade 转发设置，以及 Router 服务端实际使用的编译宏和监听端口。 |
