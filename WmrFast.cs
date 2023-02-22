using OpenCvSharp;
using OpenCvSharp.Extensions;
using Size = OpenCvSharp.Size;

namespace ClickMashine
{
    class WmrFast : Site
	{
		private Size sizeMatAuth = new(8, 10);
		private Size sizeImgClick = new(20, 26);
		WmrFastNNClick nnClick;
		private int CheckImageCompareColor(Mat bitmap)
		{
			for (int i = 0; i < bitmap.Cols; i++)
			{
				for (int j = 0; j < bitmap.Rows; j++)
				{
					Vec3b color = bitmap.At<Vec3b>(j, i);
					if (color.Item0 > 210 && color.Item1 == 0 && color.Item2 == 0)
					{
						return 2;
					}
					else if (color.Item0 == 0 && color.Item1 > 210 && color.Item2 == 0)
					{
						return 1;
					}
					else if (color.Item0 == 0 && color.Item1 == 0 && color.Item2 > 210)
					{
						return 0;
					}
				}
			}
			return -1;
		}
		private Mat WmrFastClickGray(Mat mat)
		{
			int type = CheckImageCompareColor(mat);
			Mat gray = new Mat();
			gray.Create(mat.Size(), MatType.CV_8UC1);
			for (int i = 0; i < mat.Rows; i++)
			{
				for (int j = 0; j < mat.Cols; j++)
				{
					Vec3b color = mat.At<Vec3b>(i, j);
					if (type == 0)
					{
						if (color.Item0 == 0 && color.Item1 == 0 && color.Item2 > 210)
							gray.At<byte>(i, j) = 255;
						else gray.At<byte>(i, j) = 0;
					}
					else if (type == 1)
					{
						if (color.Item0 == 0 && color.Item1 > 210 && color.Item2 == 0) gray.At<int>(i, j) = 255;
						else gray.At<byte>(i, j) = 0;
					}
					else if (type == 2)
					{
						if (color.Item0 > 210 && color.Item1 == 0 && color.Item2 == 0) gray.At<int>(i, j) = 255;
						else gray.At<byte>(i, j) = 0;
					}
				}
			}
			return gray;
		}
		public WmrFast(Form1 form, TeleBot teleBot, Auth auth) : base(form, teleBot, auth)
		{
			homePage = "https://wmrfast.com/";
			type.enam = EnumTypeSite.WmrFast;
			nnClick = new WmrFastNNClick(sizeImgClick, @"C:/ClickMashine/Settings/Net/WmrFast/WmrFastClick.h5");
		}
		public override bool Auth(Auth auth)
		{
			LoadPage(0, "https://wmrfast.com/");
			Sleep(2);
			while (true)
			{
				string ev = SendJSReturn(0, "var but_log = document.querySelector('#logbtn'); if(but_log != null) {but_log.click(); 'login';} else 'end';");
				if (ev == "login")
				{
					Sleep(2);

					WmrFastNNAuth authNN = new WmrFastNNAuth(sizeMatAuth, @"C:/ClickMashine/Settings/Net/WmrFast/WmrFastAuth.h5");
					Mat authMat = BitmapConverter.ToMat(GetImgBrowser(browsers[0].MainFrame, "document.querySelector('#login_cap')"));
					Cv2.Split(authMat, out Mat[] mat_channels);
					authMat = mat_channels[0];
					Cv2.Threshold(authMat, authMat, 140, 255, ThresholdTypes.BinaryInv);

					MatControl matControl = new MatControl(authMat);

					matControl.SplitImage(sizeMatAuth.Width,sizeMatAuth.Height, Cout: 5);

                    string js =
                            @"document.querySelector('#vhusername').value = '" + auth.Login + @"';
										document.querySelector('#vhpass').value = '" + auth.Password + @"';
										document.querySelector('#cap_text').value = '" + matControl.Predict(authNN) + @"';
										document.querySelector('#vhod1').click();";
                    eventLoadPage.Reset();
                    SendJS(0, js);
                    eventLoadPage.WaitOne();
                    Sleep(3);
                }
				else
					break;
			}
			return true;
		}
		public override void StartSurf()
		{
			Initialize();
			if (!Auth(auth))
				return;
			try
			{
				ClickSurf();
			}
			catch (Exception ex)
			{
				Error("Ошибка Click: " + ex.Message);
			}
			try
			{
				VisitSurf();
			}
			catch (Exception ex)
			{
                Error("Ошибка Visit: " + ex.Message);
			}
			try
			{
				YouTubeSurf();
			}
			catch (Exception ex)
			{
				Error("Ошибка YouTube: " + ex.Message);
			}
			CloseAllBrowser();
		}
		private void YouTubeSurf()
		{
			LoadPage(0, "https://wmrfast.com/serfing_ytn.php");
			string js =
@"var surf_cl = document.querySelectorAll('.serf_hash');var n = 0;		
function click_s()
{
	if (n >= surf_cl.length) return 'end';
	else
	{
		surf_cl[n].click(); n++; return 'surf';
	}
}";
			SendJS(0, js);
			while (true)
			{
				string ev = SendJSReturn(0, "click_s();");
				if (ev == "surf")
				{
					Sleep(3);
					if (browsers.Count == 2)
					{
						ev = SendJSReturn(1, "vs = true;timer.toString();");
						if (ev != "error")
						{
							Sleep(ev);
							WaitButtonClick(browsers[1].MainFrame, "document.querySelector('a');");
							Sleep(2);
						}
					}
				}
				else if (ev == "end")
					break;
				CloseСhildBrowser();
			}
		}
		private void ClickSurf()
		{
			LoadPage("https://wmrfast.com/serfing.php");
			string js =
@"var surf_cl = document.querySelectorAll('.serf_hash');var n = 0;		
function click_s()
{
	if (n >= surf_cl.length) return 'end';
	else
	{
		surf_cl[n].click(); n++; return 'surf';
	}
}";
			SendJS(0, js);
			while (true)
			{
				string ev = SendJSReturn(0, "click_s();");
				if (ev == "surf")
				{
					Sleep(3);
					if (browsers.Count == 2)
					{
						ev = SendJSReturn(1, "counter.toString();");
						if (ev != "error")
						{
							Sleep(ev);
							js =
@"function waitCounter(){
	if(counter == -1) return 'ok';
	else return 'wait';
}";
							ev = WaitFunction(browsers[1].MainFrame, "waitCounter();", js);
							if (ev == "ok")
							{
								for (int i = 0; i < 5; i++)
								{
									Sleep(1);
									Mat image = BitmapConverter.ToMat(GetImgBrowser(browsers[1].MainFrame, "document.querySelector('#captcha-image')"));
									Mat matClick = WmrFastClickGray(image);
									MatControl matControl = new MatControl(matClick);
									if (matControl.SplitImage(sizeImgClick.Width, sizeImgClick.Height, range: 7))
									{
										string value = matControl.Predict(nnClick);
										js = @"function endClick() {var butRet = document.querySelectorAll('[method=""POST""]');
for (var i = 0; i < butRet.length; i++)
{
	if (butRet[i].querySelector('.submit').value == " + value + @")
	{ butRet[i].querySelector('.submit').click(); return 'ok'}
}
return 'errorClick';}endClick();";
										ev = SendJSReturn(1, js);
										if (ev == "ok") { 
											Sleep(2); 
											break; 
										}
									}
									SendJS(1, "document.querySelector('#capcha > tbody > tr > td:nth-child(1) > a').click();");
									Sleep(2);
								}
							}
						}
					}
				}
				else if (ev == "end")
					break;
				CloseСhildBrowser();
				Sleep(2);
			}
		}	
		private void VisitSurf()
        {
			LoadPage("https://wmrfast.com/serfing.php");
			string js =
@"var surf_cl = document.querySelectorAll('.serf_hash');var n = 0;		
function click_s()
{
	if (n >= surf_cl.length) return surf_cl[0].getAttribute('timer').toString();
	else
	{
		surf_cl[n].click(); n++; return 'surf';
	}
}";
			SendJS(0, js);
			while (true)
			{
				string ev = SendJSReturn(0, "click_s();");
				if (ev == "end")
					break;
                else
                {
					WaitCreateBrowser(1);
					Sleep(2);
					Sleep(ev);
				}
				CloseСhildBrowser();
				Sleep(2);
			}
		}
	}
}