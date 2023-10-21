set SOURCE=..\..\..\ET
set DESTINATION=..\..\..\GameDevelopmentKit

rd/s /q %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\ModelView
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\Model
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\HotfixView
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\Hotfix

xcopy %SOURCE%\Unity\Assets\Scripts\ModelView %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\ModelView /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Model %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\Model /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\HotfixView %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\HotfixView /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Hotfix %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\Hotfix /s /e /i

pause