using ClickMashine;
using ClickMashine.Models;
using System.ComponentModel;
using ClickMashine.Sites.Profitcentr;
using ClickMashine.Sites.Aviso;

public class AutoClicker
{
    private readonly MainForm _form;
    private readonly List<Site> _siteList = new();
    private IEnumerable<AuthData> auths;
    private readonly List<Task> _tasks = new();
    public AutoClicker(MainForm form, IEnumerable<AuthData> authData)
    {
        auths = authData;
        _form = form;
        //_tcpControl.MessageReceived += TCPControl_MessageReceived;
        //_tcpControl.StartListing();

        LoadSites();
    }

    private void LoadSites()
    {
        try
        {
            if (!auths.Any()) return;
          
            foreach (var auth in auths)
            {
               switch(auth.Site)
               {
                    case EnumTypeSite.Aviso:
                        {
                            _siteList.Add(new Aviso(_form, auth));
                            break;
                        }
               }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading site: {ex.Message}");
        }
    }

    private void TCPControl_MessageReceived(object? sender, EventArgTCPClient e)
    {
        var site = _siteList.FirstOrDefault(s => s.Type == e.Message.Site);
        if (site != null)
        {
            site.TCPMessageManager.SetMessage(e.Message);
        }
        else
        {
            Console.WriteLine($"[Error] unknow site: {e.Message.Site}");
        }
    }

    public void ClickSurfAsync()
    {
        foreach (var site in _siteList)
        {
            _tasks.Add( site.Start());
        }
        Task.WaitAll( _tasks );
    }

    public void Close()
    {
        foreach (var site in _siteList)
        {
            site.Stop();
        }
    }
}
