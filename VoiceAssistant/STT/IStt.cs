using VoiceAssistant.STT.VoskStt;

namespace VoiceAssistant.STT;

public interface IStt
{
    public bool StartRecognition(byte[] buffer, int length);
    public VoskResult GetRecognitionResult();
}