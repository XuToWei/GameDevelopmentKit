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

set DESTINATION=..\..\Unity\Assets\Scripts\Game\Hot\Code\Runtime

rd/s /q %DESTINATION%\Definition\DataStruct
rd/s /q %DESTINATION%\Definition\Enum
rd/s /q %DESTINATION%\Entity\EntityData
rd/s /q %DESTINATION%\Entity\EntityLogic
rd/s /q %DESTINATION%\Game
rd/s /q %DESTINATION%\HPBar
rd/s /q %DESTINATION%\Scene
rd/s /q %DESTINATION%\UI
rd/s /q %DESTINATION%\Utility

del %DESTINATION%\HPBar.meta
del %DESTINATION%\Entity\EntityExtension.cs
del %DESTINATION%\Procedure\ProcedurePreload.cs
del %DESTINATION%\Procedure\ProcedureMenu.cs
del %DESTINATION%\Procedure\ProcedureMain.cs
del %DESTINATION%\Procedure\ProcedureGame.cs
del %DESTINATION%\Procedure\ProcedureChangeScene.cs

set DESTINATION=..\..\Unity\Assets

rd/s /q %DESTINATION%\Scripts\Game\ET
del %DESTINATION%\Scripts\Game\ET.meta
del %DESTINATION%\Res\Editor\Config\ResourceRuleEditor_ET.asset
del %DESTINATION%\Res\Editor\Config\ResourceRuleEditor_ET.meta

set DESTINATION=..\..\Design

rd/s /q %DESTINATION%\Excel\ET
rd/s /q %DESTINATION%\Proto\ET-Client
rd/s /q %DESTINATION%\Proto\ET-ClientServer

pause