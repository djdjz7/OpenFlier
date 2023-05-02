namespace OpenFlier.Services
{
    public enum MqttMessageType : long
    {
        TestReq = 30000L,
        TestResp = 30001L,
        TeacherScreenCast = 10000L,
        StopScreenCast = 10001L,
        StudentScreenCast = 10002L,
        StudentTopic = 10003L,
        StudentListReq = 10004L,
        ScreenCaptureReq = 10005L,
        DeviceVerificationReq = 10006L,
        ScreenCastUserListReq = 10008L,
        FullScreenCastReq = 10009L,
        StopSomeoneScreenCastReq = 10010L,
        DeviceVerificationResp = 20007L,
        TeacherScreenCastResp = 20000L,
        AutoStopScreenCast = 20001L,
        StudentTopicResp = 20003L,
        StudentListResp = 20004L,
        ScreenCaptureResp = 20005L,
        TeacherScreenCastError = 20010L,
        ScreenCastPortNotice = 20011L,
        StartScreenCastResp = 20012L,
        StopScreenCastResp = 20013L
    }

    public class MqttMessage<T>
    {
        public long Type { get; set; }
        public T? Data { get; set; }
    }

}
