using System;
using System.IO;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.Agent)]
    public class Admin2Agent_DeployFileHandler : MessageHandler<Scene, Admin2Agent_DeployFileRequest, Admin2Agent_DeployFileResponse>
    {
        protected override async UniTask Run(Scene scene, Admin2Agent_DeployFileRequest request, Admin2Agent_DeployFileResponse response)
        {
            try
            {
                var targetDir = Path.GetDirectoryName(request.TargetPath);
                if (!string.IsNullOrEmpty(targetDir))
                    Directory.CreateDirectory(targetDir);

                var isChunk = request.FileName.Contains(".chunk");
                if (isChunk)
                {
                    var mode = request.FileName.EndsWith(".chunk0") ? FileMode.Create : FileMode.Append;
                    using var fs = new FileStream(request.TargetPath, mode, FileAccess.Write);
                    await fs.WriteAsync(request.FileData);
                    Log.Info($"Chunk written: {request.FileName} -> {request.TargetPath} ({request.FileData.Length} bytes)");
                }
                else
                {
                    var tmpPath = $"{request.TargetPath}.tmp";
                    if (File.Exists(tmpPath))
                    {
                        using (var fs = new FileStream(tmpPath, FileMode.Append, FileAccess.Write))
                        {
                            await fs.WriteAsync(request.FileData);
                        }
                        File.Move(tmpPath, request.TargetPath, overwrite: true);
                        Log.Info($"Chunked deploy complete: {request.FileName} -> {request.TargetPath}");
                    }
                    else
                    {
                        await File.WriteAllBytesAsync(request.TargetPath, request.FileData);
                        Log.Info($"Deployed file: {request.FileName} -> {request.TargetPath} ({request.FileData.Length} bytes)");
                    }
                }
            }
            catch (Exception e)
            {
                response.Error = 1;
                response.Message = $"Failed to deploy {request.FileName}: {e.Message}";
                Log.Error(e);
            }
        }
    }
}
