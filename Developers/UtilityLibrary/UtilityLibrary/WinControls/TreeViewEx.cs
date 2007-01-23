using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using UtilityLibrary.Win32;
using UtilityLibrary.General;
using UtilityLibrary;


namespace UtilityLibrary.WinControls
{
	/// <summary>
	/// TreeView with multi-selection
	/// </summary>
	public class TreeViewEx : System.Windows.Forms.TreeView, IDisposable
	{
    #region Class Private Members
    private IList m_SelectedItems = new ArrayList(); //(int)hItem data stored
    private bool m_selectionChanged = true;
    private IList selectionChache = new ArrayList();                   
    
//    private TVHITTESTINFO m_lastHitTest;
    private IntPtr m_HitTestData;

    #endregion

    #region Class Properties
    /// <summary>
    /// Gets the copy of the list of TreeNodes, that are currently selected.
    /// </summary>
    [Browsable( false )]
    public IList SelectedNodes
    {
      get
      {
        if ( m_selectionChanged || ( selectionChache.Count != m_SelectedItems.Count ) )
        {
          selectionChache.Clear();

          foreach ( int data in m_SelectedItems )
          {
            TreeNode node = TreeNode.FromHandle( this, (IntPtr)data );
            selectionChache.Add( node );
          }

          m_selectionChanged = false;
        }

        return selectionChache;
      }
    }
    #endregion

    #region Class Delegates
    public delegate void SelectionChangedEventHandler( IList CurrentSelection );
//    public delegate void ItemSelectionChangedEventHandler( TreeNode Item, bool Selected );
    #endregion

    #region Class Events
    [Category("Behavior")]
    public event SelectionChangedEventHandler SelectionChanged;
/*    [Category("Behavior")]
    public event ItemSelectionChangedEventHandler ItemSelectionChanged;       */

    [Category("DragDrop")]
    public event CustomDragger.DataRequestEventHandler  DraggerDataRequest;
    [Category("DragDrop")]
    public event CustomDragger.DropEffectsEventHandler  DraggerOverEfectRequest;
    [Category("DragDrop")]
    public event CustomDragger.DropDataEventHandler     DraggerDrop;
    #endregion
    
    #region Class Initialization/Finalization
    public TreeViewEx()
    {
    }

    new public void Dispose()
    {
      base.Dispose();

      if ( m_HitTestData != IntPtr.Zero )
        Marshal.FreeHGlobal( m_HitTestData );
    }

    #endregion
    
    #region Class Property Changes Handling
    protected virtual void OnSelectionChanged()
    {
      if ( SelectionChanged != null )
        SelectionChanged( SelectedNodes );
    }
    #endregion
    
    #region Control`s Event Handling
    protected override  void WndProc(ref Message message)
		{
      bool oldLabelEdit = LabelEdit;
      switch (message.Msg)
			{
        case (int)Msg.WM_LBUTTONDOWN:
          POINT p = new POINT( (int)message.LParam );
          TVHITTESTINFO ht = HitTest( p );
          
          if ( ht.hItem != IntPtr.Zero && !m_SelectedItems.Contains( (int)ht.hItem ) )
          {
            LabelEdit = false;
            SelectTreeItem( ht.hItem );
          }
          break;

        // Reflected Messages come from the treeview control itself
				case (int)ReflectedMessages.OCM_NOTIFY:
					NMHDR nm2 = (NMHDR) message.GetLParam(typeof(NMHDR));	
					switch (nm2.code)
					{
						case (int)NotificationMessages.NM_CUSTOMDRAW:
              base.WndProc(ref message);
							NotifyTreeCustomDraw(ref message); 
              return;

            case (int)TreeViewNotification.TVN_DeleteItem:
              NMTREEVIEW tv = (NMTREEVIEW) message.GetLParam(typeof(NMTREEVIEW));	
              
              if ( m_SelectedItems.Contains( (int)tv.itemOld.hItem ) )
              {
                m_SelectedItems.Remove( (int)tv.itemOld.hItem );
                m_selectionChanged =true;
                OnSelectionChanged();
              }

              break;
            
            case (int)TreeViewNotification.TVN_SelectionChanged:
              NMTREEVIEW tv1 = (NMTREEVIEW) message.GetLParam(typeof(NMTREEVIEW));	
              int hItem = (int)tv1.itemNew.hItem;

              if ( (WindowsAPI.GetKeyState( (int)VirtualKeys.VK_SHIFT ) | 1) != 1 )
              {
                if ( !m_SelectedItems.Contains( hItem ) )
                  m_SelectedItems.Add( hItem );
                else
                  m_SelectedItems.Remove( hItem );
              }
              else if ( (WindowsAPI.GetKeyState( (int)VirtualKeys.VK_CONTROL ) | 1) != 1 )
              {
                if ( !m_SelectedItems.Contains( hItem ) )
                  m_SelectedItems.Add( hItem );
                else
                  m_SelectedItems.Remove( hItem );
              }
              else
              {
                bool bFullInvalidate = m_SelectedItems.Count > 1;

                m_SelectedItems.Clear();
                m_SelectedItems.Add( hItem );

                if ( bFullInvalidate )
                  Invalidate();
              } 
              
              m_selectionChanged = true;
              OnSelectionChanged();
              break;

          default:
            break;
        }
          break;

        default:
          break;
      }

      base.WndProc(ref message);
      LabelEdit = oldLabelEdit;
    }

    #endregion
    
    #region Custom Drawing
    private bool NotifyTreeCustomDraw(ref Message m)
    {
      m.Result = (IntPtr)CustomDrawReturnFlags.CDRF_DODEFAULT;
      NMTVCUSTOMDRAW tvcd = (NMTVCUSTOMDRAW)m.GetLParam(typeof(NMTVCUSTOMDRAW));
      IntPtr thisHandle = Handle;
      
      if ( tvcd.nmcd.hdr.hwndFrom != Handle)
        return false;

			switch (tvcd.nmcd.dwDrawStage)
			{
        case (int)CustomDrawDrawStateFlags.CDDS_ITEMPREERASE:
          m.Result = (IntPtr)CustomDrawReturnFlags.CDRF_SKIPDEFAULT;
          break;
				case (int)CustomDrawDrawStateFlags.CDDS_PREPAINT:
					// Ask for Item painting notifications
					m.Result = (IntPtr)CustomDrawReturnFlags.CDRF_NOTIFYITEMDRAW;
					break;      

        case (int)CustomDrawDrawStateFlags.CDDS_ITEMPREPAINT:
          bool bSelected = m_SelectedItems.Contains( tvcd.nmcd.dwItemSpec );
          tvcd.clrTextBk =  ( bSelected ) ? ColorUtil.RGB( SystemColors.Highlight ) : ColorUtil.RGB( SystemColors.Window );
          tvcd.clrText   =  ( bSelected ) ? ColorUtil.RGB( SystemColors.HighlightText ) : ColorUtil.RGB( SystemColors.WindowText );
                                          
                                         
          // Put structure back in the message
					Marshal.StructureToPtr(tvcd, m.LParam, true);
					m.Result = (IntPtr)CustomDrawReturnFlags.CDRF_NOTIFYPOSTPAINT;
					break;
				
        default:
          break;

      }
      return false;
    }

    #endregion

    #region Dragger Implementation
    private void GetData( object sender, CustomDragger.DataRequestArgs e )
    {
      if ( DraggerDataRequest != null )
        DraggerDataRequest( sender, e );
    }
    private void GetEffect( object sender, CustomDragger.DropEffectsArgs e )
    {
      if ( DraggerOverEfectRequest != null )
        DraggerOverEfectRequest( sender, e );
    }
    private void PutData( object sender, CustomDragger.DropRequestArgs e )
    {
      if ( DraggerDrop != null )
        DraggerDrop( sender, e );
    }
    #endregion

    #region Class Helpher Function
    TVHITTESTINFO HitTest( Point p )
    {
      TVHITTESTINFO htInfo = new TVHITTESTINFO();
      htInfo.pt = p;
      
      if ( m_HitTestData == IntPtr.Zero )
        m_HitTestData = Marshal.AllocHGlobal( Marshal.SizeOf( htInfo ) );

      Marshal.StructureToPtr( htInfo, m_HitTestData, true );  //Save to unmanaged memory
      WindowsAPI.SendMessage(Handle, (int)TreeViewMessages.TVM_HITTEST, IntPtr.Zero, m_HitTestData );
      htInfo = (TVHITTESTINFO)Marshal.PtrToStructure( m_HitTestData, htInfo.GetType() );        //Load from unmanaged memory
                               
      return htInfo;
    }

    void SelectTreeItem( IntPtr hItem )
    {
      WindowsAPI.SendMessage(Handle, (int)TreeViewMessages.TV_FIRST + 11, (IntPtr)9, hItem );
    }

    public void AddDraggerLink( Control control )
    {
      CustomDragger dragger = new CustomDragger( this, control );
      dragger.OnDataRequest += new CustomDragger.DataRequestEventHandler( GetData );
      dragger.OnEffectsRequest += new CustomDragger.DropEffectsEventHandler( GetEffect );
      dragger.OnDataDrop += new CustomDragger.DropDataEventHandler( PutData );
      
    }

    #endregion


	}
}
