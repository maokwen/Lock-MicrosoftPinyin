using Microsoft.Win32;
using System;
using System.Windows.Forms;
using System.Reflection;

public class ApplicationContext1 : ApplicationContext
{
  private readonly NotifyIcon notifyIcon;
  private readonly ContextMenuStrip contextMenu;
  private readonly System.Windows.Forms.Timer timer;
  private readonly uint WM_IME_CONTROL = 0x283;
  private readonly IntPtr IMC_SETCONVERSIONMODE = new IntPtr(0x002);
  private readonly IntPtr IME_CHINESE = new IntPtr(0x401);

  [System.Runtime.InteropServices.DllImport("user32.dll")]
  public static extern IntPtr GetForegroundWindow();

  [System.Runtime.InteropServices.DllImport("imm32.dll")]
  public static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);

  [System.Runtime.InteropServices.DllImport("user32.dll")]
  public static extern Int16 GetKeyboardLayout(Int32 hWnd);

  [System.Runtime.InteropServices.DllImport("user32.dll")]
  public static extern IntPtr SendMessage(IntPtr hWnd, uint wMsg, IntPtr wParam, IntPtr lParam);

  public ApplicationContext1()
  {
    contextMenu = new ContextMenuStrip();
    contextMenu.ShowImageMargin = false;
    contextMenu.Items.Add(new ToolStripMenuItem("Exit", null, OnExit));

    notifyIcon = new NotifyIcon();
    notifyIcon.Icon = SystemIcons.Application;
    notifyIcon.ContextMenuStrip = contextMenu;
    notifyIcon.Visible = true;

    timer = new System.Windows.Forms.Timer();
    timer.Tick += LockIME;
    timer.Interval = 500;
    timer.Enabled = true;
    timer.Start();
  }

  private void LockIME(object? sender, EventArgs e)
  {
    var hWnd = GetForegroundWindow();
    if (hWnd == IntPtr.Zero) return;
    var id = ImmGetDefaultIMEWnd(hWnd);
    if (id == IntPtr.Zero) return;

    if (InChsStatus())
    {
      SetChsStatus(id);
    }
  }

  private bool InChsStatus()
  {
    var langId = GetKeyboardLayout(0);
    var lang = (langId & 0xffff);
    return lang == 0x804;
  }

  private void SetChsStatus(IntPtr hWnd)
  {
    SendMessage(hWnd, WM_IME_CONTROL, IMC_SETCONVERSIONMODE, IME_CHINESE);
  }

  private void OnExit(object? sender, EventArgs e)
  {
    timer.Stop();
    notifyIcon.Dispose();
    Application.Exit();
  }
}
