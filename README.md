# VoiceAssistant
Locally hosted voice assistant with plugin extension feature

Based on the idea of https://github.com/janvarev/Irene-Voice-Assistant and the magnificient VOSK speech recognition engine (https://github.com/alphacep/vosk-api)

Basically the ideas are:
1) Locally hosted voice recognition and speech generation
2) Plugin extensibility model
3) Easily configurable commands via JSON configs for each plugin
4) Multi-language (just change settings for speech generation and model of speech recognition)

Thanks to:
- VOSK voice recognition module: https://alphacephei.com/vosk/
  language models: https://alphacephei.com/vosk/models
- RHVoice Lab voices for Windows SAPI5: https://rhvoice.su/voices/
- NAudio project: https://github.com/naudio/NAudio
- Weighting string comparison alghoritm: https://github.com/JakeBayer/FuzzySharp

ToDo:
- move core messages into resources
- command validation all over plugins to avoid similar commands
- allow command injection from plugin (enable by config parameter)

Plugins list planned:
1. Hello (done)
2. Timer (done)
3. Open web-site in browser (done)
4. Run program (done)
5. Currency rates (https://www.cbr-xml-daily.ru/daily_json.js , http://www.cbr.ru/scripts/XML_daily.asp) (done)
6. Application control using key code injection (suitable for MPC-HC, VLC, Foobar2000, etc)

7. Google/Yandex calendar tasks check/add (https://developers.google.com/calendar/api/quickstart/dotnet)
8. Play music from folder by name/artist (foobar - https://www.foobar2000.org/components/view/foo_beefweb , https://hyperblast.org/beefweb/api/)

9. Weather check (yandex)
10. Suburban trains (yandex)
11. Message broadcast/announce to selected/all instances in the network (websocket + mqtt)
12. Voice connection (interphone/speakerphone) between instances (websocket + mqtt)

