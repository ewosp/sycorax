using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;

using UtilityLibrary.Win32;


namespace UtilityLibrary.Dialogs
{
  [ ToolboxItem( true ),
  ToolboxBitmap( typeof( UtilityLibrary.Dialogs.FolderSelectDialog ), 
    "UtilityLibrary.Dialogs.FolderSelectDialog.bmp" ) ]
  public class FolderSelectDialog : CommonDialog
  {
    #region Class constants
    private const string DEF_TITLE_TEXT = "Please Select Folder";
    #endregion

    #region Class members
    private bool m_bBrowseForComputer;
    private bool m_bBrowseForPrinter;
    private bool m_bIncludeFiles;
    private bool m_bIncludeUrls;
    private bool m_bDontGoBelowDomain;
    private bool m_bEditBox;
    private bool m_bNewDialogStyle;
    private bool m_bNoNewFolderButton;
    private bool m_bNoTranslateTargets;
    private bool m_bReturnOnlyFileSystemAncestors;
    private bool m_bReturnOnlyFileSystemDirs;
    private bool m_bShareable;
    private bool m_bStatusText;
    private bool m_bUsageHint;
    private bool m_bUseNewUI;
    private bool m_bValidate;

    private string m_sTitle = DEF_TITLE_TEXT;
    private string m_sFullName;
    #endregion
    
    #region Class Properties
    /// <summary>
    /// Path, that was selected
    /// </summary>
    [Browsable( false )]
    public string ResultPath
    {
      get
      {
        return m_sFullName;
      }
    }
    
    /// <summary>
    /// Title of the window, that will be shown
    /// </summary>
    [ Category( "Appearance" ), 
    DefaultValue( DEF_TITLE_TEXT ), 
    Description( "GET/SET Dialog`s title text." ) ]
    public string Title
    {
      get
      {
        return m_sTitle;
      }
      set
      {
        if ( value != m_sTitle )
        {
          m_sTitle = value;
        }
      }
    }
    
    /// <summary> 
    ///Only return computers. If the user selects anything other than a computer,
    /// the OK button is grayed. </summary>
    [ Category( "Behavior" ), 
    DefaultValue( false ), 
    Description( "Only return computers. If the user selects anything other than a computer, the OK button is grayed." ) ]
    public bool BrowseForComputer
    {
      get
      {
        return m_bBrowseForComputer;
      }
      set
      {
        if ( value != m_bBrowseForComputer )
        {
          m_bBrowseForComputer = value;
        }
      }
    }

    /// <summary> 
    ///Only return printers. If the user selects anything other than a printer, the
    /// OK button is grayed.</summary>
    [ Category( "Behavior" ), 
    DefaultValue( false ), 
    Description( "Only return printers. If the user selects anything other than a printer, the OK button is grayed." ) ]
    public bool BrowseForPrinter
    {
      get
      {
        return m_bBrowseForPrinter;
      }
      set
      {
        if ( value!= m_bBrowseForPrinter)
        {
          m_bBrowseForPrinter = value;
        }
      }
    }

    /// <summary> The browse dialog box will display files as well as folders.
    /// </summary>
    [ Category( "Behavior" ), 
    DefaultValue( false ), 
    Description( "The browse dialog box will display files as well as folders." ) ]
    public bool IncludeFiles
    {
      get
      {
        return m_bIncludeFiles;
      }
      set
      {
        if ( value != m_bIncludeFiles )
        {
          m_bIncludeFiles = value;
        }
      }
    }

    /// <summary> 
    /// The browse dialog box can display URLs. The BIF_USENEWUI and
    /// BIF_BROWSEINCLUDEFILES flags must also be set. If these three flags are not
    /// set, the browser dialog box will reject URLs. Even when these flags are set,
    /// the browse dialog box will only display URLs if the folder that contains the 
    /// selected item supports them. When the folder's IShellFolder::GetAttributesOf
    /// method is called to request the selected item's attributes, the folder must
    /// set the SFGAO_FOLDER attribute flag. Otherwise, the browse dialog box will
    /// not display the URL. </summary>
    [ Category( "Behavior" ), 
    DefaultValue( false ), 
    Description( "The browse folder dialog box can display URLs." ) ]
    public bool IncludeUrls
    {
      get
      {
        return m_bIncludeUrls;
      }
      set
      {
        if ( value != m_bIncludeUrls )
        {
          m_bIncludeUrls = value;
        }
      }
    }

    /// <summary> Do not include network folders below the domain level in the 
    /// dialog box's tree view control. </summary>
    [ Category( "Behavior" ), 
    DefaultValue( false ), 
    Description( "Do not include network folders below the domain level in the dialog box's tree view control." ) ]
    public bool DontGoBelowDomain
    {
      get
      {
        return m_bDontGoBelowDomain;
      }
      set
      {
        if ( value != m_bDontGoBelowDomain )
        {
          m_bDontGoBelowDomain = value;

        }
      }
    }

    /// <summary> Include an edit control in the browse dialog box that allows the 
    /// user to type the name of an item. </summary>
    [ Category( "Behavior" ), 
    DefaultValue( false ), 
    Description( "Include an edit control in the browse dialog box that allows the user to type the name of an item." ) ]
    public bool EditBox
    {
      get
      {
        return m_bEditBox;
      }
      set
      {
        if ( value != m_bEditBox )
        {
          m_bEditBox = value;
        }
      }
    }

    /// <summary> Use the new user interface. Setting this flag provides the user 
    /// with a larger dialog box that can be resized. The dialog box has several new 
    /// capabilities including: drag and drop capability within the dialog box, 
    /// reordering, shortcut menus, new folders, delete, and other shortcut menu 
    /// commands. </summary>
    [ Category( "Behavior" ), 
    DefaultValue( false ), 
    Description( "Use the new user interface." ) ]
    public bool NewDialogStyle
    {
      get
      {
        return m_bNewDialogStyle;
      }
      set
      {
        if ( value != m_bNewDialogStyle )
        {
          m_bNewDialogStyle = value;
        }
      }
    }

    /// <summary> Do not include the New Folder button in the browse dialog box.
    /// </summary>
    [ Category( "Behavior" ), 
    DefaultValue( false ), 
    Description( "Do not include the New Folder button in the browse dialog box." ) ]
    public bool NoNewFolderButton
    {
      get
      {
        return m_bNoNewFolderButton;
      }
      set
      {
        if ( value != m_bNoNewFolderButton )
        {
          m_bNoNewFolderButton = value;
        }
      }
    }

    /// <summary> When the selected item is a shortcut, return the PIDL of the 
    /// shortcut itself rather than its target. </summary>
    [ Category( "Behavior" ), 
    DefaultValue( false ), 
    Description( "When the selected item is a shortcut, return the PIDL of the shortcut itself rather than its target." ) ]
    public bool NoTranslateTargets
    {
      get
      {
        return m_bNoTranslateTargets;
      }
      set
      {
        if ( value != m_bNoTranslateTargets )
        {
          m_bNoTranslateTargets = value;
        }
      }
    }

    /// <summary> Only return file system ancestors. An ancestor is a subfolder that 
    /// is beneath the root folder in the namespace hierarchy. If the user selects an 
    /// ancestor of the root folder that is not part of the file system, the OK button 
    /// is grayed. </summary>
    [ Category( "Behavior" ), 
    DefaultValue( false ), 
    Description( "Only return file system ancestors." ) ]
    public bool ReturnOnlyFileSystemAncestors
    {
      get
      {
        return m_bReturnOnlyFileSystemAncestors;

      }
      set
      {
        if ( value != m_bReturnOnlyFileSystemAncestors ) 
        {
          m_bReturnOnlyFileSystemAncestors = value;
        }
      }
    }

    /// <summary> Only return file system directories. If the user selects folders 
    /// that are not part of the file system, the OK button is grayed. </summary>
    [ Category( "Behavior" ), 
    DefaultValue( false ), 
    Description( "Only return file system directories." ) ]
    public bool ReturnOnlyFileSystemDirs
    {
      get
      {
        return m_bReturnOnlyFileSystemDirs;
      }
      set
      {
        if ( value != m_bReturnOnlyFileSystemDirs )
        {
          m_bReturnOnlyFileSystemDirs = value;
        }
      }
    }

    /// <summary> The browse dialog box can display shareable resources on remote 
    /// systems. It is intended for applications that want to expose remote shares on a 
    /// local system. The BIF_USENEWUI flag must also be set. </summary>
    [ Category( "Behavior" ), 
    DefaultValue( false ), 
    Description( "The browse dialog box can display shareable resources on remote systems." ) ]
    public bool Shareable
    {
      get
      {
        return m_bShareable;
      }
      set
      {
        if ( value != m_bShareable )
        {
          m_bShareable = value;
        }
      }
    }

    /// <summary> Include a status area in the dialog box. The callback function can 
    /// set the status text by sending messages to the dialog box. </summary>
    [ Category( "Behavior" ), 
    DefaultValue( false ), 
    Description( "Include a status area in the dialog box." ) ]
    public bool StatusText
    {
      get
      {
        return m_bStatusText;
      }
      set
      {
        if ( value != m_bStatusText )
        {
          m_bStatusText = value;
        }
      }
    }

    /// <summary> When combined with BIF_NEWDIALOGSTYLE, adds a usage hint to the 
    /// dialog box in place of the edit box. BIF_EDITBOX overrides this flag. </summary>
    [ Category( "Behavior" ), 
    DefaultValue( false ), 
    Description( "Show usage hint." ) ]
    public bool UsageHint
    {
      get
      {
        return m_bUsageHint;
      }
      set
      {
        if ( value != m_bUsageHint )
        {
          m_bUsageHint = value;
        }
      }
    }

    /// <summary> Use the new user interface, including an edit box. This flag is 
    /// equivalent to BIF_EDITBOX | BIF_NEWDIALOGSTYLE. </summary>
    [ Category( "Behavior" ), 
    DefaultValue( false ), 
    Description( "Use the new user interface, including an edit box." ) ]
    public bool UseNewUI
    {
      get
      {
        return m_bUseNewUI;
      }
      set
      {
        if ( value != m_bUseNewUI )
        {
          m_bUseNewUI = value;
        }
      }
    }

    /// <summary> If the user types an invalid name into the edit box, the browse 
    /// dialog box will call the application's BrowseCallbackProc with the 
    /// BFFM_VALIDATEFAILED message. This flag is ignored if BIF_EDITBOX is not 
    /// specified. </summary>
    [ Category( "Behavior" ), 
    DefaultValue( false ), 
    Description( "If the user types an invalid name into the edit box, the browse dialog box will call the application's BrowseCallbackProc with the BFFM_VALIDATEFAILED message." ) ]
    public bool Validate
    {
      get
      {
        return m_bValidate;
      }
      set
      {
        if ( value != m_bValidate )
        {
          m_bValidate = value;
        }
      }
    }
    #endregion
                                      
    #region Class Controls
    private System.ComponentModel.Container components = null;
    #endregion

    #region Class Initialization/Finalization
    public FolderSelectDialog(System.ComponentModel.IContainer container)
    {
      container.Add( this );
      InitializeComponent();
    }

    public FolderSelectDialog()
    {
      InitializeComponent();
    }
    #endregion

    #region Component Designer generated code
    private void InitializeComponent()
    {
      components = new System.ComponentModel.Container();
    }
    #endregion

    #region Class Functions
    private UInt32 GetFlagsValue()
    {
      UInt32 flags = 0;

      if( BrowseForComputer )             flags |= (uint)BrowseInfoFlag.BIF_BROWSEFORCOMPUTER;
      if( BrowseForPrinter )              flags |= (uint)BrowseInfoFlag.BIF_BROWSEFORPRINTER;
      if( IncludeFiles )                  flags |= (uint)BrowseInfoFlag.BIF_BROWSEINCLUDEFILES;
      if( IncludeUrls )                   flags |= (uint)BrowseInfoFlag.BIF_BROWSEINCLUDEURLS;
      if( DontGoBelowDomain )             flags |= (uint)BrowseInfoFlag.BIF_DONTGOBELOWDOMAIN;
      if( EditBox )                       flags |= (uint)BrowseInfoFlag.BIF_EDITBOX;
      if( NewDialogStyle )                flags |= (uint)BrowseInfoFlag.BIF_NEWDIALOGSTYLE;
      if( NoNewFolderButton )             flags |= (uint)BrowseInfoFlag.BIF_NONEWFOLDERBUTTON;
      if( NoTranslateTargets )            flags |= (uint)BrowseInfoFlag.BIF_NOTRANSLATETARGETS;
      if( ReturnOnlyFileSystemAncestors ) flags |= (uint)BrowseInfoFlag.BIF_RETURNFSANCESTORS;
      if( ReturnOnlyFileSystemDirs )      flags |= (uint)BrowseInfoFlag.BIF_RETURNONLYFSDIRS;
      if( Shareable )                     flags |= (uint)BrowseInfoFlag.BIF_SHAREABLE;
      if( StatusText )                    flags |= (uint)BrowseInfoFlag.BIF_STATUSTEXT;
      if( UsageHint )                     flags |= (uint)BrowseInfoFlag.BIF_UAHINT;
      if( UseNewUI )                      flags |= (uint)BrowseInfoFlag.BIF_USENEWUI;
      if( Validate )                      flags |= (uint)BrowseInfoFlag.BIF_VALIDATE;

      return flags;
    }
    
    private IShellFolder GetDesktopFolder()
    {
      IntPtr ptrRet;
      WindowsAPI.SHGetDesktopFolder( out ptrRet );

      System.Type shellFolderType = System.Type.GetType( "ShellLib.IShellFolder" );
      object obj = Marshal.GetTypedObjectForIUnknown( ptrRet, shellFolderType );
      IShellFolder ishellFolder = ( IShellFolder )obj;

      return ishellFolder;
    }
    
    private Int32 myBrowseCallbackProc(IntPtr hwnd, UInt32 uMsg, Int32 lParam, Int32 lpData)
    {
      return 0;
    }
    #endregion

    #region Class Overrides
    protected override bool RunDialog( System.IntPtr hwndOwner )
    {
      string m_DisplayName = "";
      string sDisplay;
      IntPtr ptrRet;

      // Get IMalloc interface
      WindowsAPI.SHGetMalloc( out ptrRet );

      Type    mallocType = System.Type.GetType( "IMalloc" );
      IMalloc pMalloc = ( IMalloc )Marshal.GetTypedObjectForIUnknown( ptrRet, mallocType );
      IntPtr  pidlRoot = IntPtr.Zero;

      BROWSEINFO bi = new BROWSEINFO();
    
      bi.hwndOwner  = hwndOwner;    
      bi.pidlRoot   = pidlRoot;     //TODO: Root`s selection
      bi.pszDisplayName = new string( ' ', 256 );
      bi.lpszTitle  = Title;
      bi.ulFlags    = GetFlagsValue(); 
      bi.lParam     = 0;
      bi.lpfn       = new WindowsAPI.BrowseCallbackProc( this.HookProc );

      IntPtr pidlSelected;
      pidlSelected = WindowsAPI.SHBrowseForFolder( ref bi );
      m_DisplayName = bi.pszDisplayName.ToString();

      // if display name is whitespace then return FAIL
      if( m_DisplayName.Trim() == string.Empty ) return false;

      IShellFolder isf = GetDesktopFolder();

      STRRET ptrDisplayName;
      isf.GetDisplayNameOf( pidlSelected,
        (uint)SHGNO.SHGDN_NORMAL | (uint)SHGNO.SHGDN_FORPARSING,
        out ptrDisplayName );
    
      WindowsAPI.StrRetToBSTR( ref ptrDisplayName, pidlRoot, out sDisplay );
      m_sFullName = sDisplay;
      
      if( pidlRoot != IntPtr.Zero )
        pMalloc.Free( pidlRoot );
    
      if( pidlSelected != IntPtr.Zero )
        pMalloc.Free( pidlSelected );
    
      Marshal.ReleaseComObject( isf );
      Marshal.ReleaseComObject( pMalloc );
      
      return true;
    }

    public override void Reset()
    {
      m_bBrowseForComputer = false;
      m_bBrowseForPrinter = false;
      m_bIncludeFiles = false;
      m_bIncludeUrls = false;
      m_bDontGoBelowDomain = false;
      m_bEditBox = false;
      m_bNewDialogStyle = false;
      m_bNoNewFolderButton = false;
      m_bNoTranslateTargets = false;
      m_bReturnOnlyFileSystemAncestors = false;
      m_bReturnOnlyFileSystemDirs = false;
      m_bShareable = false;
      m_bStatusText = false;
      m_bUsageHint = false;
      m_bUseNewUI = false;
      m_bValidate = false;

      m_sTitle = DEF_TITLE_TEXT;
      m_sFullName = string.Empty;
    }
    
    #endregion

  }
}
