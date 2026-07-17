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
                var targetPath = ResolveTargetPath(request.TargetPath);
                var targetDir = Path.GetDirectoryName(targetPath);
                if (!string.IsNullOrEmpty(targetDir))
                    Directory.CreateDirectory(targetDir);

                if (TryGetChunkIndex(request.FileName, out var chunkIndex))
                {
                    var mode = chunkIndex == 0 ? FileMode.Create : FileMode.Append;
                    using var fs = new FileStream(targetPath, mode, FileAccess.Write, FileShare.None);
                    await fs.WriteAsync(request.FileData);
                    Log.Info($"Chunk written: {request.FileName} -> {targetPath} ({request.FileData.Length} bytes)");
                }
                else if (request.FileName == AgentActorService.DeployFinalChunkMarker)
                {
                    var tmpPath = $"{targetPath}.tmp";
                    if (!File.Exists(tmpPath))
                    {
                        throw new InvalidDataException($"Missing partial deployment for {targetPath}");
                    }

                    using (var fs = new FileStream(tmpPath, FileMode.Append, FileAccess.Write, FileShare.None))
                    {
                        await fs.WriteAsync(request.FileData);
                    }
                    File.Move(tmpPath, targetPath, overwrite: true);
                    Log.Info($"Chunked deploy complete: {targetPath}");
                }
                else
                {
                    var tmpPath = $"{targetPath}.tmp";
                    if (File.Exists(tmpPath))
                    {
                        File.Delete(tmpPath);
                    }

                    await File.WriteAllBytesAsync(targetPath, request.FileData);
                    Log.Info($"Deployed file: {request.FileName} -> {targetPath} ({request.FileData.Length} bytes)");
                }
            }
            catch (Exception e)
            {
                response.Error = 1;
                response.Message = $"Failed to deploy {request.FileName}: {e.Message}";
                Log.Error(e);
            }
        }

        private static string ResolveTargetPath(string requestedPath)
        {
            if (string.IsNullOrWhiteSpace(requestedPath))
            {
                throw new ArgumentException("Target path is required", nameof(requestedPath));
            }

            var deploymentRoot = Path.GetFullPath(AppContext.BaseDirectory);
            var targetPath = Path.GetFullPath(requestedPath, deploymentRoot);
            var relativePath = Path.GetRelativePath(deploymentRoot, targetPath);
            if (relativePath == ".." ||
                relativePath.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal) ||
                Path.IsPathRooted(relativePath))
            {
                throw new UnauthorizedAccessException("Deploy target must stay inside the Agent application directory");
            }

            return targetPath;
        }

        private static bool TryGetChunkIndex(string fileName, out int chunkIndex)
        {
            chunkIndex = -1;
            if (string.IsNullOrEmpty(fileName) ||
                !fileName.StartsWith(AgentActorService.DeployChunkPrefix, StringComparison.Ordinal))
            {
                return false;
            }

            return int.TryParse(fileName.AsSpan(AgentActorService.DeployChunkPrefix.Length), out chunkIndex) &&
                    chunkIndex >= 0;
        }
    }
}
