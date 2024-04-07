set DESTINATION=..\..\Unity\Assets\Res

rd/s /q %DESTINATION%\Art
rd/s /q %DESTINATION%\Material
rd/s /q %DESTINATION%\Mesh
rd/s /q %DESTINATION%\Music
rd/s /q %DESTINATION%\Prefab
rd/s /q %DESTINATION%\Scene
rd/s /q %DESTINATION%\Sound
rd/s /q %DESTINATION%\Texture
rd/s /q %DESTINATION%\UI\UIForm
rd/s /q %DESTINATION%\UI\UIPrefab
rd/s /q %DESTINATION%\UI\UISound
rd/s /q %DESTINATION%\UI\UISprite

set DESTINATION=..\..\Unity\Assets\Scripts\Game\ET\Code

xcopy %DESTINATION%\Hotfix\Client\Demo\NetClient %DESTINATION%\Hotfix\Client\Module\NetClient /s /e /i
xcopy %DESTINATION%\Model\Client\Demo\NetClient %DESTINATION%\Model\Client\Module\NetClient /s /e /i

copy %DESTINATION%\Hotfix\Client\Demo\NetClient.meta %DESTINATION%\Hotfix\Client\Module\NetClient.meta
copy %DESTINATION%\Model\Client\Demo\NetClient.meta %DESTINATION%\Model\Client\Module\NetClient.meta

mkdir %DESTINATION%\Model\Client\Module\Main
mkdir %DESTINATION%\Hotfix\Client\Module\Main
copy %DESTINATION%\Model\Client\Demo\Main.meta %DESTINATION%\Model\Client\Module\Main.meta
copy %DESTINATION%\Hotfix\Client\Demo\Main.meta %DESTINATION%\Hotfix\Client\Module\Main.meta

copy %DESTINATION%\Model\Client\Demo\Main\ClientSenderComponent.cs %DESTINATION%\Model\Client\Module\Main\ClientSenderComponent.cs
copy %DESTINATION%\Model\Client\Demo\Main\ClientSenderComponent.cs.meta %DESTINATION%\Model\Client\Module\Main\ClientSenderComponent.cs.meta
copy %DESTINATION%\Hotfix\Client\Demo\Main\ClientSenderComponentSystem.cs %DESTINATION%\Hotfix\Client\Module\Main\ClientSenderComponentSystem.cs
copy %DESTINATION%\Hotfix\Client\Demo\Main\ClientSenderComponentSystem.cs.meta %DESTINATION%\Hotfix\Client\Module\Main\ClientSenderComponentSystem.cs.meta
copy %DESTINATION%\Hotfix\Client\Demo\Main\NetClient2Main_SessionDisposeHandler.cs %DESTINATION%\Hotfix\Client\Module\Main\NetClient2Main_SessionDisposeHandler.cs
copy %DESTINATION%\Hotfix\Client\Demo\Main\NetClient2Main_SessionDisposeHandler.cs.meta %DESTINATION%\Hotfix\Client\Module\Main\NetClient2Main_SessionDisposeHandler.cs.meta

rd/s /q %DESTINATION%\Hotfix\Client\Demo
rd/s /q %DESTINATION%\Hotfix\Client\LockStep
rd/s /q %DESTINATION%\Hotfix\Client\Test
rd/s /q %DESTINATION%\Hotfix\Server\Benchmark
rd/s /q %DESTINATION%\Hotfix\Server\Demo
rd/s /q %DESTINATION%\Hotfix\Server\LockStep

del %DESTINATION%\Hotfix\Client\Demo.meta
del %DESTINATION%\Hotfix\Client\LockStep.meta
del %DESTINATION%\Hotfix\Client\Test.meta
del %DESTINATION%\Hotfix\Server\Benchmark.meta
del %DESTINATION%\Hotfix\Server\Demo.meta
del %DESTINATION%\Hotfix\Server\LockStep.meta

rd/s /q %DESTINATION%\HotfixView\Client\Demo
rd/s /q %DESTINATION%\HotfixView\Client\LockStep

del %DESTINATION%\HotfixView\Client\Demo.meta
del %DESTINATION%\HotfixView\Client\LockStep.meta

rd/s /q %DESTINATION%\Model\Client\Demo
rd/s /q %DESTINATION%\Model\Client\LockStep
rd/s /q %DESTINATION%\Model\Client\Test
rd/s /q %DESTINATION%\Model\Server\Benchmark
rd/s /q %DESTINATION%\Model\Server\Demo
rd/s /q %DESTINATION%\Model\Server\LockStep

del %DESTINATION%\Model\Client\Demo.meta
del %DESTINATION%\Model\Client\LockStep.meta
del %DESTINATION%\Model\Client\Test.meta
del %DESTINATION%\Model\Server\Benchmark.meta
del %DESTINATION%\Model\Server\Demo.meta
del %DESTINATION%\Model\Server\LockStep.meta

rd/s /q %DESTINATION%\ModelView\Client\Demo
rd/s /q %DESTINATION%\ModelView\Client\LockStep

del %DESTINATION%\ModelView\Client\Demo.meta
del %DESTINATION%\ModelView\Client\LockStep.meta

set DESTINATION=..\..\Unity\Assets\Scripts\Game\Hot\Code\Runtime

rd/s /q %DESTINATION%\Definition\DataStruct
rd/s /q %DESTINATION%\Definition\Enum
rd/s /q %DESTINATION%\Entity\EntityData
rd/s /q %DESTINATION%\Entity\EntityLogic
rd/s /q %DESTINATION%\Game
rd/s /q %DESTINATION%\HPBar
rd/s /q %DESTINATION%\Module
rd/s /q %DESTINATION%\Scene
rd/s /q %DESTINATION%\UI
rd/s /q %DESTINATION%\Utility

del %DESTINATION%\HPBar.meta

pause