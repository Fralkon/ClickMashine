namespace ClickMashine.Models
{
    public class SettingBrowser
    {
        public required string UserAgent { get; set; }
        public required string AcceptLanguageList { get; set; }
        public required IEnumerable<AuthData> Auths { get; set; }
    }
}
