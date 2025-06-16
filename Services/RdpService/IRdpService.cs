namespace GraphicRdpScopeToggler.Services.RdpService
{
    public interface IRdpService
    {
        public void OpenRdpForAll();
        public void CloseRdpForAll();
        public void OpenRdpForLocalComputers();
        public int? GetRdpPort();
        public bool IsRdpPortOpen(int port);
    }
}
