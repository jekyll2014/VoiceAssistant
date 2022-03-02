# VoiceAssistant
Locally hosted voice assistant with plugin extension feature

Based on the idea of https://github.com/janvarev/Irene-Voice-Assistant and based on the magnificient VOSK speech recognition engine (https://github.com/alphacep/vosk-api)

Basically the ideas are:
1) Locally hosted voice recognition and speech generation
2) Plugin extensibility model
3) Easily configurable commands via JSON configs for each plugin
4) Multi-language (just change settings for speech generation and model of speech recognition)

Bugs:
- exception on trying to play any .wav file async
- all strings are hard-coded

ToDo:
- move string into resources
- command waiting end signal (bip sound)
- MQTT connection for instance-to-instance command exchange
- command validation all over plugins to avoid similar commands
- check audio capture can be used by plugin

Plugins list planned:
1. Hello
2. Timer
3. MPC-HC (VLC?) control with web interface
4. Google/Yandex calendar task add
5. Weather check
6. Suburban trains (yandex)
7. Currency rates
8. Open web-site in browser
9. Run program
10. Message broadcast/announce to selected/all instances in the network (websocket or json + mqtt)
11. Voice connection (interphone/speakerphone) between instances
