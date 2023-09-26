using CefSharp;
using System;
using Tensorflow;

namespace ClickMashine
{
    enum ProfiCentrEnumNN
    {
        ёжиками,
        автомобили,
        апельсинами,
        арбузами,
        бананами,
        бантиками,
        велосипедами,
        девушками,
        деньгами,
        деревьями,
        домами,
        жирафами,
        карандашами,
        клавиатурами,
        книгами,
        котятами,
        лампочками,
        лодками,
        лошадьми,
        медалями,
        мышками,
        носками,
        пандой,
        поездами,
        поросятами,
        самолетами,
        скрипками,
        слонами,
        собаками,
        стульями,
        телефонами,
        тиграми,
        цветами,
        цифрами,
        чайниками,
        экскаваторами,
		яблоками
	}
    class Profitcentr : Site
	{
		public Profitcentr(Form1 form, Auth auth) : base(form, auth)
		{
			homePage = "https://profitcentr.com/";
			Type = EnumTypeSite.Profitcentr;
		}
		public override bool Auth(Auth auth)
		{
			var browserAuth = GetBrowser(0);
			if(browserAuth == null) { return false; }
			LoadPage(0, "https://profitcentr.com/login");
			string auth_js = "document.querySelector('input[name=\"username\"]').value = '" + auth.Login + "';" +
							 "document.querySelector('input[name=\"password\"]').value = '" + auth.Password + "';";
            SendJS(0, auth_js);
			if (AuthorizationAntiBot(browserAuth))
			{
				string ev = GetMoney(browserAuth, "document.querySelector('#new-money-ballans')");
				if (ev == "error")
					return false;
                siteStripComboBox.Text = StatusSite.online.ToString();
                return true;
			}
			return false;
		}
		protected override void StartSurf()
		{
			Initialize();




            if (!Auth(auth))
				waitHandle.WaitOne();

            mSurf.AddFunction(ClickSurf);
            mSurf.AddFunction(MailSurf);
            mSurf.AddFunction(VisitSurf);
            mSurf.AddFunction(YouTubeSurf);
            mSurf.AddFunction(RuTubeSurf);
            while (true)
			{
				mSurf.GoSurf();
				Sleep(600);
			}
		}
		private int ClickSurf()
		{
			int Count = 0;
			var mainBrowser = GetBrowser(0);
			if(mainBrowser == null) { return -1; }
			LoadPage("https://profitcentr.com/");
            LoadPage(SendJSReturn(mainBrowser, "document.querySelector('#mnu_tblock1 > a:nth-child(1)').href"));

            if (!OutCaptchaLab(mainBrowser, "document.querySelector('.out-capcha')", "document.querySelectorAll('.out-capcha-inp')", "document.querySelector('.btn').click();"))
            {
                Error("Error captcha youtube");
                return Count;
            }

            string js =
@"var surf_cl = document.querySelectorAll('.work-serf');var n = 0;
function surf()
{
	var start_ln = surf_cl[n].querySelector('.butt-yes-test');
	if (start_ln != null) { start_ln.click(); n++; return 'surf'; }
	else { return 'wait'; }
}
function click_s()
{
	if (n >= surf_cl.length) return 'end_surf';
	else
	{	
		if(surf_cl[n].querySelector('.workquest')!=null)
			{
				if(surf_cl[n].querySelectorAll('td')[2].querySelector('a')==null || surf_cl[n].getBoundingClientRect().height == 0)
					{n++; return 'continue';}
				else {surf_cl[n].querySelector('a').click(); return 'click';}
			}
		else
			{n++;return 'continue';}
	}
}";
			SendJS(0, js);
			while (true)
			{
				eventBrowserCreated.Reset();
				string ev = SendJSReturn(0, "click_s();");
				if (ev == "end_surf")
					break;
				else if (ev == "continue")
					continue;
				else if (ev == "click")
				{
					for (int i = 0; i < 5; i++)
					{
						ev = SendJSReturn(0, "surf();");
						if (ev == "wait")
							Sleep(1);
						else if (ev == "surf")
						{
							IBrowser? browserSurf = WaitCreateBrowser();
							if (browserSurf != null)
							{
								if (WaitElement(browserSurf, "frminfo", "document.querySelector('#timer_inp')"))
                                {
                                    IFrame frame = browserSurf.GetFrame("frminfo");
                                    ev = SendJSReturn(frame,
@"b = false;
window.top.start = 0;
var timer_s = document.querySelector('#timer_inp');
if (timer_s != null)
	timer_s.innerText;
else 'error_surf';");
									if (ev != "error_surf")
									{
										Sleep(ev);
										if (WaitElement(browserSurf, "frminfo", @"document.querySelector('[type=""range""]')"))
                                        {
                                            frame = browserSurf.GetFrame("frminfo");
                                            SendJSReturn(frame,
	@"var range = document.querySelector('[type=""range""]');
if (range != null)
{
	range.value = range.max;
	document.querySelector('button').click();
	'end';
}
else
{
	location.replace(""vlss?view=ok"");
	'error_surf';
}");
											Count++;
											Sleep(2);
										}
										break;
									}
								}
							}
                        }
					}
				}
				CloseСhildBrowser();
                Sleep(2);
            }
			return Count;
		}
		private int VisitSurf()
        {
			int Count = 0;
			IBrowser mainBrowser = GetBrowser(0);
			if (mainBrowser == null) return -1;
			LoadPage(mainBrowser, "https://profitcentr.com/");
            LoadPage(SendJSReturn(mainBrowser, "document.querySelector('#mnu_tblock1 > a:nth-child(2)').href"));
            if (!OutCaptchaLab(mainBrowser, "document.querySelector('.out-capcha')", "document.querySelectorAll('.out-capcha-inp')", "document.querySelector('.btn').click();"))
            {
                Error("Error captcha youtube");
                return Count;
            }

            string js =
@"var surf_cl = document.querySelectorAll('.work-serf');var n = 0;
function click_s()
{
	if (n >= surf_cl.length) return 'end_surf';
	else
	{
		var link = surf_cl[n];
		if(link.querySelectorAll('td')[2].querySelector('a')==null || link.getBoundingClientRect().height == 0)
					{n++; return 'continue';}
		else {link.querySelector('a').click(); n++; return link.querySelectorAll('td')[1].querySelectorAll('div')[1].innerText;}
	}
}";
			SendJS(mainBrowser, js);
			while (true)
			{
				eventBrowserCreated.Reset();
				string ev = SendJSReturn(mainBrowser, "click_s();");
				if (ev == "end_surf")
					break;
				else if (ev == "continue")
					continue;
				else
				{
					if (WaitCreateBrowser() != null)
					{
						int pointStart = ev.IndexOf("Таймер: ") + 8;
						int pointEnd = ev.IndexOf(' ', pointStart);
						int countText = pointEnd - pointStart;
						if (pointStart == -1 || pointEnd == -1 || countText > 0)
						{
							Count++;
							Sleep(ev.Substring(pointStart, pointEnd - pointStart));
						}
						Sleep(2);
					}
                }
				CloseСhildBrowser();
                Sleep(2);
            }
			return Count;
		}
		private int YouTubeSurf()
		{
			int Count = 0;
			var mainBrowser = GetBrowser(0);
			if (mainBrowser == null) return -1;
			LoadPage(SendJSReturn(mainBrowser, "document.querySelector('#mnu_tblock1 > a:nth-child(6)').href"));
            if (!OutCaptchaLab(mainBrowser, "document.querySelector('.out-capcha')", "document.querySelectorAll('.out-capcha-inp')", "document.querySelector('.btn').click();"))
            {
                Error("Error captcha youtube");
                return Count;
            }

            IFrame main_frame = mainBrowser.MainFrame;
			SendJS(main_frame, @"var loadPage = document.querySelector('#load-pages');
if(loadPage != null) loadPage.click();");
			Sleep(2);

			for (int i = 0; i < 5; i++)
			{
				string goBottom = SendJSReturn(main_frame, @"var goBottom = document.querySelector('#Go_Bottom');
if(goBottom.style.display != ""none"") {goBottom.click(); 'reply';}
else 'end';");
				Sleep(2);
				if (goBottom == "end")
					break;
			}
			string js_links = @"var surf_cl = document.querySelectorAll('.work-serf');var n = 0;
			function click_s()
			{
				if (n >= surf_cl.length) return 'end_surf';
				else
				{
					surf_cl[n].querySelector('span').click(); return 'click';
				}
			}
			function surf()
			{
				var start_ln = surf_cl[n].querySelector('.youtube-button');
				if (start_ln != null) { 
					if(start_ln.innerText != 'Приступить к просмотру') {n++; return 'continue';}
					else {start_ln.querySelector('span').click(); n++; return 'surf'; }
				}
				else { return 'sec_wait'; }
			}";
			SendJS(main_frame, js_links);
			while (true)
			{
				eventBrowserCreated.Reset();
				string ev = SendJSReturn(main_frame, "click_s();");
				if (ev == "end_surf")
				{
					break;
				}
				else if (ev == "click")
				{
					for (int i = 0; i < 10; i++)
					{
						ev = SendJSReturn(main_frame, "surf();");
						if (ev == "surf")
						{
							var browserYouTube = WaitCreateBrowser();
							if (browserYouTube != null)
							{
								IFrame yotube_frame = browserYouTube.MainFrame;
								ev = SendJSReturn(yotube_frame,
	@"c = true;  b = true; document.querySelector('#tmr').innerText;");
								if (ev != "error")
								{
									Sleep(ev);
									if (WaitButtonClick(yotube_frame, "document.querySelector('.butt-nw');"))
										Count++;
									Sleep(2);
								}
							}
							break;
						}
						else if (ev == "sec_wait")
							Sleep(1);
						else if (ev == "continue")
							break;
						if(i == 9)
						{
							SendJS(main_frame, "n++;");
						}
					}
				}
				else
				{
					Sleep(1);
				}
				CloseСhildBrowser();
				Sleep(1);
			}
			return Count;
		}
		private int RuTubeSurf()
		{
			int Count = 0;
			IBrowser mainBrowser = GetBrowser(0);
			if (mainBrowser == null) return -1;
            LoadPage("https://profitcentr.com/");
            LoadPage(SendJSReturn(mainBrowser, "document.querySelector('#mnu_tblock1 > a:nth-child(7)').href"));
            if (!OutCaptchaLab(mainBrowser, "document.querySelector('.out-capcha')", "document.querySelectorAll('.out-capcha-inp')", "document.querySelector('.btn').click();"))
            {
                Error("Error captcha youtube");
                return Count;
            }

            IFrame main_frame = mainBrowser.MainFrame;
            SendJS(main_frame, @"var loadPage = document.querySelector('#load-pages');
if(loadPage != null) loadPage.click();");
            Sleep(2);

            for (int i = 0; i < 5; i++)
            {
                string goBottom = SendJSReturn(main_frame, @"var goBottom = document.querySelector('#Go_Bottom');
if(goBottom.style.display != ""none"") {goBottom.click(); 'reply';}
else 'end';");
                Sleep(1);
                if (goBottom == "end")
                    break;
            }
            string js_links = @"var surf_cl = document.querySelectorAll('.work-serf');var n = 0;
			function click_s()
			{
				if (n >= surf_cl.length) return 'end_surf';
				else
				{
					surf_cl[n].querySelector('span').click(); return 'click';
				}
			}
			function surf()
			{
				var start_ln = surf_cl[n].querySelector('.youtube-button');
				if (start_ln != null) { 
					if(start_ln.innerText != 'Приступить к просмотру') {n++; return 'continue';}
					else {start_ln.querySelector('span').click(); n++; return 'surf'; }
				}
				else { return 'sec_wait'; }
			}";
            SendJS(main_frame, js_links);
            while (true)
            {
                eventBrowserCreated.Reset();
                string ev = SendJSReturn(main_frame, "click_s();");
                if (ev == "end_surf")
                {
                    break;
                }
                else if (ev == "click")
                {
                    for (int i = 0; i < 10; i++)
                    {
                        ev = SendJSReturn(main_frame, "surf();");
                        if (ev == "surf")
                        {
                            var browserYouTube = WaitCreateBrowser();
							if (browserYouTube != null)
							{
								IFrame youtubeFrame = browserYouTube.MainFrame;
								if(!WaitElement(youtubeFrame, "document.querySelector('#tmr')"))
								{
									Error("YouTube error");
									break;
								}
								ev = SendJSReturn(youtubeFrame,
	@"c = true; b = true; document.querySelector('#tmr').innerText;");
								if (ev != "error")
								{
									Sleep(ev);

									if (!WaitButtonClick(youtubeFrame, "document.querySelector('.butt-nw');"))
									{
										Error("Error end youtube watch");
                                        break;
                                    }
									Count++;
									Sleep(2);
								}
								break;
							}
                        }
                        else if (ev == "sec_wait")
                            Sleep(1);
                        else if (ev == "continue")
                            break;
                    }
                }
                else MessageBox.Show("Ошибка блять");
                CloseСhildBrowser();
                Sleep(1);
            }
			return Count;
		}
		private int MailSurf()
		{
			int Count = 0;
            var mainBrowser = GetBrowser(0);
			if (mainBrowser == null) return -1;
            LoadPage("https://profitcentr.com/");
            LoadPage(SendJSReturn(mainBrowser, "document.querySelector('#mnu_tblock1 > a:nth-child(3)').href"));
			string js =
@"var surf_cl = document.querySelectorAll('.work-serf');var n = 1;
function surf()
{
	var start_ln = surf_cl[n].querySelector('.butt-yes-test');
	if (start_ln != null) { start_ln.click(); n++; return 'surf'; }
	else { return 'wait'; }
}
function click_s()
{
	if (n >= surf_cl.length) { return 'end_surf';}
	else
	{
		surf_cl[n].querySelector('a').click(); return 'click';
	}
}";
			SendJS(0, js);
			while (true)
            {
                eventBrowserCreated.Reset();
                string ev = SendJSReturn(0, "click_s();");
				if (ev == "end_surf")
					break;
				else if (ev == "contionue")
					continue;
				else if (ev == "click")
				{
					for (int i = 0; i < 10; i++)
					{
						ev = SendJSReturn(0, "surf();");
						if (ev == "wait")
							Sleep(1);
						else if (ev == "surf")
						{
							if (WaitElement(mainBrowser, "document.querySelector('.taskdescription')"))
							{
								ev = GetMailAnswer(mainBrowser.MainFrame, "document.querySelector('.taskdescription')",
									"document.querySelector('.taskquestion')",
									"document.querySelectorAll('.mails-otvet-new a')");
								if (ev == "errorMail")
								{
									Random rnd = new Random();
									ev = rnd.Next(0, 3).ToString();
								}
								SendJS(0, "document.querySelectorAll('.mails-otvet-new a')[" + ev + "].click();");
								IBrowser? browserMail = WaitCreateBrowser();
								if (browserMail != null)
								{
									ev = SendJSReturn(browserMail,
	@"b = false;
var timer1 = document.querySelector('#tmr');
if (timer1 != null)
	timer1.innerText;
else 'error_surf';");
									if (ev != "error")
									{
										Sleep(ev);
										if (WaitElement(browserMail, @"document.querySelector('[type=""range""]')"))
										{
											string function =
	@"var range = document.querySelector('[type=""range""]');
if (range != null)
{
	range.value = range.max;
	document.querySelector('button').click();
	'end';
}
else
{
	'error';
}";
											ev = SendJSReturn(browserMail, function);
											if (ev != "error")
											{
												Count++;
												Sleep(2);
											}
										}
									}
								}
							}
						}
					}
				}
				CloseСhildBrowser();
			}
			return Count;
		}
		private bool AuthorizationAntiBot(IBrowser browser)
        {
            ProfitcentNN nn = new ProfitcentNN(@"C:/Users/Boyarkin/Desktop/Profitcentr.h5");
            BoundObject boundObject = new BoundObject();
            for (int i = 0; i < 5; i++)
			{
				string jsAntiBot =
	@"var captha_lab = document.querySelectorAll('.out-capcha-lab');
if(captha_lab.length != 0){
    'captcha';
}
else 'ok';";
				string evAntiBot = SendJSReturn(browser, jsAntiBot);
				CM(evAntiBot);
				if (evAntiBot == "ok")
					return true;
				else if (evAntiBot == "error")
				{
					CM("ERROR");
					Error("Ошибка капчи");
					return false;
				}
				else
				{
                    SendJS(browser, "document.querySelectorAll('.out-capcha-lab').forEach((element) => element.style.border = '0px');");
                    Sleep(1);
					ProfiCentrEnumNN? enumNN = null;
					string nameImage = SendJSReturn(browser, "document.querySelector('.out-capcha-title').innerText;");
                    foreach (string item in Enum.GetNames(typeof(ProfiCentrEnumNN)))
                    {
						if(nameImage.IndexOf(item) != -1)
						{
							enumNN = Enum.Parse<ProfiCentrEnumNN>(item);
							break;
						}
                    }
					if (enumNN != null)
                    {
						List<Tuple<Bitmap, PredictNN>> imageHistoryPredict = new List<Tuple<Bitmap, PredictNN>>();
						for (int j = 0; j < 6; j++)
						{
							Bitmap image = GetImgBrowser(browser.MainFrame, "document.querySelectorAll('.out-capcha-lab')[" + j.ToString() + "]");
							PredictNN predict = nn.Predict(image);
                            imageHistoryPredict.Add(Tuple.Create(image, predict));
							if (enumNN == (ProfiCentrEnumNN)predict.Num)
								SendJS(browser, "document.querySelectorAll('.out-capcha-inp')[" + j + "].checked = true;");
						}
						WaitChangeElement(browser, boundObject, "document.querySelector('.login-error')");
						boundObject.ResetEvent();
                        SendJS(browser, "document.querySelector('.btn_big_green').click();");
						string ev = boundObject.GetValue();
                        if (ev == "error")
						{
                            SaveHistoryCaptcha(imageHistoryPredict, enumNN);
                        }
						else if (ev == "Нужно подтвердить, что Вы не робот!")
						{
                            SaveHistoryCaptcha(imageHistoryPredict, enumNN);
                        }
						else if (ev == "Ваш аккаунт заблокирован")
						{
							AccountBlock();
							return false;
						}
						else
							return true;
						//Bitmap img = GetImgBrowser(browsers[0].MainFrame, "document.querySelector('.out-capcha')");
						//string answer_telebot = SendQuestion(img, "");
						//if (answer_telebot == "")
						//	jsAntiBot = "";
						//foreach (char ch in answer_telebot)
						//	jsAntiBot += "document.querySelectorAll('.out-capcha-inp')[" + ch + "].checked = true;";
						//jsAntiBot += "document.querySelector('.btn_big_green').click();";
					}
					SendJS(browser, jsAntiBot);
					Sleep(10);
				}
			}
			Error("Ошибка ввода капчи");
			return false;
		}
        private bool OutCaptchaLab(IBrowser browser, string captcha, string input, string button)
        {
            ProfitcentNN nn = new ProfitcentNN(@"C:/Users/Boyarkin/Desktop/Profitcentr.h5");
            string js =
@"var img_captcha = " + captcha + @";
if(img_captcha != null)
    'antiBot';
else 'notAntiBot';";
            int iteration = 0;
            while (SendJSReturn(browser.MainFrame, js) == "antiBot")
            {
                if (iteration == 10)
                    return false;

                string jsAntiBot = String.Empty;
                string answerBot = SendQuestion(GetImgBrowser(browser.MainFrame, captcha), "");
                if (answerBot != "upload")
                {
                    foreach (char ch in answerBot)
                        jsAntiBot += input + "[" + ch + "].checked = true;";
                    jsAntiBot += button + ";";
                    SendJS(browser.MainFrame, jsAntiBot);
                    Sleep(4);
                }
                iteration++;
                eventLoadPage.Reset();
                browser.Reload();
                eventLoadPage.WaitOne(5000);
            }
            return true;
        }
		//private void SaveHistoryCaptcha(List<Tuple<Bitmap, PredictNN>> historyCaptcha, ProfiCentrEnumNN? enumNN)
		//{
		//	string path = $"{Form1.PATH_SETTING}Errors/{DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss")}/";
		//	Directory.CreateDirectory(path);
		//	for (int i = 0; i < historyCaptcha.Count; i++)
  //          {
  //              string file = enumNN.ToString() + Environment.NewLine + "----------------------------" + Environment.NewLine;
  //              Tuple<Bitmap, PredictNN>? captcha = historyCaptcha[i];
		//		captcha.Item1.Save(path + i.ToString() + ".png");
		//		var tensor = captcha.Item2.Tensor.ToArray<float>();
		//		for (int j = 0; j < tensor.Length; j++)
		//			file += ((ProfiCentrEnumNN)j).ToString() + " : " + tensor[j].ToString() + Environment.NewLine;
		//		File.WriteAllText(path + $"debug{i.ToString()}.txt", file);
		//	}
		//}
        //     private void TrainBD()
        //     {
        //         ProfitcentNN nn = new ProfitcentNN(@"C:/Users/Boyarkin/Desktop/Profitcentr.h5");
        //LoadPage("https://profitcentr.com/login");
        //         string path = @"C:\ClickMashine\Settings\Image\" + Type.ToString() + @"\";
        //         for (int i = 0; i < 100; i++) {
        //	string name = SendJSReturn(Browsers[0].MainFrame, "document.querySelector('.out-capcha-title').innerText");
        //	string[] name_item = name.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //	Directory.CreateDirectory(path + name_item[name_item.Length-1] + @"\");

        //             SendJS(0, "document.querySelectorAll('.out-capcha-lab').forEach((element) => element.style.border = '0px');");
        //	Sleep(1);
        //             for (int j = 0; j < 6; j++)
        //	{

        //		PredictNN predict = nn.Predict(GetImgBrowser(Browsers[0].MainFrame, "document.querySelectorAll('.out-capcha-lab')[" + j.ToString() + "]"));
        //		foreach (var v in predict.Tensor.numpy())
        //		{
        //			foreach (var v2 in v)
        //				Console.WriteLine(v2.ToString());
        //		}
        //		Console.WriteLine((ProfiCentrEnumNN)predict.Num);
        //		Console.ReadLine();

        //		//              GetImgBrowser(browsers[0].MainFrame, "document.querySelectorAll('.out-capcha-lab')[" + j.ToString() + "]")
        //		//.Save(path + new DirectoryInfo(path).GetFiles().Length.ToString() + ".png");
        //	}
        //	SendJS(0, "document.querySelector('.out-reload').click();");
        //	Sleep(2);
        //}
        //     }
    }
}