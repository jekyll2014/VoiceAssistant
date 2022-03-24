# VoiceAssistant
Locally hosted voice assistant with plugin extension feature

Based on the idea of https://github.com/janvarev/Irene-Voice-Assistant and the magnificient VOSK speech recognition engine (https://github.com/alphacep/vosk-api)

### Basically the ideas are:
1) Locally hosted voice recognition and speech generation
2) Plugin extensibility model
3) Easily configurable commands via JSON configs for each plugin
4) Multi-language (just change settings for the speech generation and model of speech recognition, than rewrite plugin commands to your language)
5) Almost no CPU load on low power Intel Core i5 3rd generation. Should even work on Intel Atom.

### Thanks to:
- VOSK voice recognition module: https://alphacephei.com/vosk/
  language models: https://alphacephei.com/vosk/models
- RHVoice Lab voices for Windows SAPI5: https://rhvoice.su/voices/
- NAudio project: https://github.com/naudio/NAudio
- Weighting string comparison alghoritm: https://github.com/JakeBayer/FuzzySharp

### ToDo:
- move core messages into resources
- command validation all over plugins to avoid similar commands
- allow command injection from plugin (enable by config parameter)
- refactor settings names

### Plugins list:
- Hello
- Timer (done)
- Open web-site in browser (done)
- Run program (done)
- Currency rates (https://www.cbr-xml-daily.ru/daily_json.js , http://www.cbr.ru/scripts/XML_daily.asp) (done)
- Application control using key code injection (suitable for MPC-HC, VLC, Foobar2000, etc)

### Plugins list planned:
- Google/Yandex calendar tasks check/add (https://developers.google.com/calendar/api/quickstart/dotnet)
- Play music from folder by name/artist (foobar - https://www.foobar2000.org/components/view/foo_beefweb , https://hyperblast.org/beefweb/api/)
- Weather check (yandex?)
- Suburban trains (yandex?)
- Message broadcast/announce to selected/all instances in the network (websocket + mqtt)
- Voice connection (interphone/speakerphone) between instances (websocket + mqtt)

## Core module settings are in the appsettings.json file:
-  "ModelFolder": "model" - folder with VOSK voice model. Get ti from official site https://alphacephei.com/vosk/models
  
-  "SelectedAudioInDevice": "" - which audio device to listen to. The names are the same as in the Windows sound device manager and are printed out on start up. The comparison is done with .StartWith() so the name doesn't have to be full
  
-  "SelectedAudioOutDevice": "" - which audio device to play sounds at. The list is printed out on start up.

-  "AudioInSampleRate": 16000 - no need to touch. Recommended settings.

-  "AudioOutSampleRate": 44100 - no need to touch. Doesn't really change the quality.

-  "CallSign": [ "Baby" ] - list of the call signs to wake assistant up. Could be one or several. Could have several similar versions of one word if you found the recognition misses too often.

-  "DefaultSuccessRate": 90 - default success rate for soft string complarison. Not used really since all the commands should have it.

-  "VoiceName": "" - voice model name for TextToSpeech. The list is printed out on start up.

-  "SpeakerCulture": "en-US" - if the VoiceName is not set this culture will be used to select any suitable voice model for TextToSpeech. 

-  "PluginsFolder": "plugins" - folder with plugins. No restrictions on names or folder structure. It simply searches for plugin file mask and then plugin get it's settings from the setting file wit autogenerated name like %pluginName%Settings.json and all the resources are loaded from plugin current folder.

-  "PluginFileMask": "*Plugin.dll" - mask of the plugin files.

-  "StartSound": "AssistantStart.wav" - start up sound. Just for fun.

-  "MisrecognitionSound": "Misrecognition.wav" - warning sound to note that the command wasn't found.

-  "CommandAwaitTime": 10 - time to wait for a command phrase starting from the first callsign detection.

-  "NextWordAwaitTime": 3 - time to wait for a next word in the command phrase.

-  "VoskLogLevel": -1 - VOSK debug setting. No need to touch.

-  "CommandNotRecognizedMessage": "Command not recognized" - the phrase to inform user accordingly.

-  "CommandNotFoundMessage": "Command not found" - the phrase to inform user accordingly.

-  "AllowPluginsToListenToSound": true - plugins MAY have (none of mines uses it) an access to the microphone any time they wish. If you don't need it - turn it off for safety sake.

-  "AllowPluginsToListenToWords": true - plugins MAY have (none of mines uses it) an access to the recogized word stream any time they wish. If you don't need it - turn it off for safety sake.

## The plugins settings are different for every single plugin but there are common parts recommended^
  // this is a definition of all commands available for the plugin
  "Commands": [
  
    // this is a very short command
    {
      "Response": "Hi",  // this is a custom field but I think most of the commands should have the same to inform user about success command execution
      "Name": "Greeting short informal", // just a name of the command. I'm not using it for the processing but it's handy for JSON editing
      "Tokens": [  // here I define a sequence of words for the command phrase
        {
          "Value": [  // a set of interchangeable words.
            "Hi",
            "Hello"
          ],
          "Type": "Command",  // each token could be a "Command" or "Parameter"
          "SuccessRate": 90  // success comparison rate for soft comparison
        }
      ]
    }
  
  // and here is more complicated command with parameters for the timer
    {
      "Response": "{1} minutes {2} seconds timer stopped",  // response string has {x} marks to put data to before pronouncing it. This is up to plugin developer to use them and user must follow the developer recomendations using them.
      "isStopCommand": true,  // this is a custom parameter I use to separate STOP commands from START commands. I found it handy for development of this exact plugin but it's just the way I did here.
      "Name": "Stop timer minutes, seconds",  // just a name for readability
      "Tokens": [
      // this is a usual command token. Nothing to add here.
        {
          "Value": [
            "stop",
            "delete"
          ],
          "Type": "Command",
          "SuccessRate": 90
        },
        
        // and here is the parameter token. The words will be added to it until we mee next command word. So the parameter could receive multiple words on processing.
        {
        // I use value name to get the value out of phrase and use it properly. The %% brackets are not necessary but I feel it more readable.
          "Value": [
            "%minutes%"  // do not mix the parameter names or make it the same - it'll break the alghorithm.
          ],
          "Type": "Parameter",  // this makes it a parameter token and program processes it in a special way.
          "SuccessRate": 90
        },
        
        // and so on...
        {
          "Value": [
            "minute",
            "minutes"
          ],
          "Type": "Command",
          "SuccessRate": 90
        },
        {
          "Value": [
            "%seconds%"
          ],
          "Type": "Parameter",
          "SuccessRate": 90
        },
        {
          "Value": [
            "second",
            "seconds"
          ],
          "Type": "Command",
          "SuccessRate": 90
        },
		{
          "Value": [
            "timer"
          ],
          "Type": "Command",
          "SuccessRate": 90
        }
      ]
    }
  ]

### Frankly speaking a developer should only provide this command structure for the core module to setup the command recognition. But the actual processing logic can be any. One can even prive cthe core with empty command definition and attach to the audio stream or text stream and process it himself.
