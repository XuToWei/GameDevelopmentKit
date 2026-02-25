using ET;
using Luban;

var builder = DistributedApplication.CreateBuilder(args);

// 读取环境变量
var sceneName = Environment.GetEnvironmentVariable("SceneName") ?? "";
var startConfig = Environment.GetEnvironmentVariable("StartConfig") ?? "Localhost";
var singleThread = int.Parse(Environment.GetEnvironmentVariable("SingleThread") ?? "0");

// 初始化Options单例
var options = new Options();
options.StartConfig = startConfig;
World.Instance.AddSingleton(options);

// 计算项目根目录 - 从程序集位置向上找到包含Config目录的目录
var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
var assemblyDir = Path.GetDirectoryName(assemblyLocation)!;
var workDir = assemblyDir;

// 向上查找直到找到Config目录
while (!Directory.Exists(Path.Combine(workDir, "Config")))
{
    var parent = Directory.GetParent(workDir);
    if (parent == null)
    {
        // 如果找不到，使用当前目录的父目录
        workDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), ".."));
        break;
    }
    workDir = parent.FullName;
}

Console.WriteLine($"Project root directory: {workDir}");

var configBasePath = Path.Combine(workDir, "Config", "Luban");
Console.WriteLine($"Reading ET configs from: {configBasePath}");

// 加载配置
World.Instance.AddSingleton<Tables>();
await Tables.Instance.LoadAsync(async file => new ByteBuf(await File.ReadAllBytesAsync(Path.Combine(configBasePath, $"{file}.bytes"))));

// 为每个进程创建Aspire服务
foreach (var startProcessConfig in Tables.Instance.DTStartProcessConfig.DataList)
{
    if (!string.Equals(startProcessConfig.StartConfig, startConfig))
    {
        continue;
    }

    var processId = startProcessConfig.Id;
    var replicasNum = 1; // 默认副本数为1

    var processScenes = Tables.Instance.DTStartSceneConfig.GetByProcess(processId);

    var innerIP = startProcessConfig.InnerIP;
    var innerPortStr = startProcessConfig.Port.ToString();

    var outerIP = startProcessConfig.OuterIP;

    var outerPortStr = "0";
    // 进程只有一个Scene, 需要设置Scene OuterPort
    if (processScenes.Count == 1)
    {
        var startSceneConfig = processScenes[0];
        outerPortStr = startSceneConfig.Port.ToString();
    }

    // 为每个副本创建独立的服务
    for (var replicaIndex = 1; replicaIndex <= replicasNum; replicaIndex++)
    {
        var serviceName = $"et-process-{processId}-{replicaIndex}";

        // workingDirectory使用Bin目录
        var binDir = Path.Combine(workDir, "Bin");
        builder.AddExecutable(serviceName, "dotnet", binDir, "App.dll")
            .WithArgs($"--Process={processId}") // 指定进程Id
            .WithArgs($"--ReplicaIndex={replicaIndex}") // 副本索引
            .WithArgs($"--SceneName={sceneName}") // 场景名
            .WithArgs($"--StartConfig={startConfig}") // 启动配置
            .WithArgs($"--SingleThread={singleThread}") // 单线程模式
            .WithEnvironment("InnerIP", innerIP)
            .WithEnvironment("InnerPort", innerPortStr)
            .WithEnvironment("OuterIP", outerIP)
            .WithEnvironment("OuterPort", outerPortStr)
            .WithOtlpExporter()
            .WithEnvironment("ASPIRE_MANAGED", "true");
    }
}

// 启动Aspire
builder.Build().Run();
