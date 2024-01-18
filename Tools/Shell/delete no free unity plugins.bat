set DESTINATION=..\..\Unity\Assets

rd/s /q %DESTINATION%\Scripts\Library\EnhancedScroller
rd/s /q "%DESTINATION%\Scripts\Library\SLATE Cinematic Sequencer"
rd/s /q %DESTINATION%\Scripts\Library\SmartUiSelection
rd/s /q %DESTINATION%\Scripts\Library\StompyRobot
rd/s /q %DESTINATION%\Scripts\Plugins\Demigiant
rd/s /q %DESTINATION%\Scripts\Plugins\Sirenix

del %DESTINATION%\Scripts\Library\EnhancedScroller.meta
del "%DESTINATION%\Scripts\Library\SLATE Cinematic Sequencer.meta"
del %DESTINATION%\Scripts\Library\SmartUiSelection.meta
del %DESTINATION%\Scripts\Library\StompyRobot.meta
del %DESTINATION%\Scripts\Plugins\Demigiant.meta
del %DESTINATION%\Scripts\Plugins\Sirenix.meta

pause