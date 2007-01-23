using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using UtilityLibrary.Win32;
using UtilityLibrary.General;


namespace UtilityLibrary.Combos
{
  [ToolboxItem(true)]
  [ToolboxBitmap(typeof(UtilityLibrary.Combos.TreeCombo), "UtilityLibrary.Combos.TreeCombo.bmp")]
  public class TreeCombo : CustomCombo
  {
    #region Class Internal declarations
    public class EventArgsTreeDataFill : CustomCombo.EventArgsBindDropDownControl
    {
      new public TreeView BindedControl
      {
        get
        {
          return (TreeView)base.BindedControl;
        }
        set
        {
          base.BindedControl = value;
        }
      }


      public EventArgsTreeDataFill( CustomCombo.EventArgsBindDropDownControl ev )
        : base( ev.Combo, ev.DropDownForm, ev.BindedControl )
      {
      }
    }

    public delegate void FillTreeByDataHandler( object sender, EventArgsTreeDataFill e );
    #endregion

    #region Class Members
    protected TreeView  m_tree = new TreeView();
    protected ImageList m_imgList;

    private   bool      m_bFillCalled;
    private   bool      m_bCloseOnClick = true;
    #endregion

    #region Class Events
    [ Category( "Action" ), 
    Description( "Raised by control when data of drop down control must be filled/set" ) ]
    public event FillTreeByDataHandler DataFill;
    
    [ Category( "Action" ),
    Description( "Raised by control when is clicked in DropDownList Area " ) ]
    public event EventHandler DropDownClicked;
    #endregion

    #region Class Properties
    [Browsable(false)]
    public TreeView   TreeDropDown
    {
      get
      { 
        BindDropDownControl();
        return m_tree; 
      }
    }
    
    [Browsable(true)]
    public ImageList  TreeImageList
    {
      get
      {
        return m_imgList;
      }
      set
      {
        if( value != m_imgList )
        {
          m_imgList = value;
          m_tree.ImageList = m_imgList;
        } 
      }
    }

    [Browsable(true)]
    public bool DropDownHotTracking
    {
      set
      {      
        if( m_tree.HotTracking != value )  
        {
          m_tree.HotTracking = value;
        }
      }
      get
      {
        return m_tree.HotTracking;
      }
    }
    
    [ Browsable( true ), DefaultValue( true ) ]
    public bool CloseOnClick
    {
      get
      {
        return m_bCloseOnClick;
      }
      set
      {
        m_bCloseOnClick = value;
      }
    }
    #endregion

    #region Class Constructos
    public TreeCombo()
    {
      m_tree.BorderStyle = BorderStyle.None;
      m_tree.ImageList = m_imgList; 
      m_tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler( OnTreeItemChanged );
      m_tree.Click += new System.EventHandler( OnDropDownClicked );
      base.CloseDropDown += new CustomCombo.CloseDropDownHandler( OnCloseDropDownHandler );
      base.CustomEditSize += new CustomCombo.EditControlResizeHandler( OnItemSizeCalculate );
    }
    #endregion

    #region Class Overrides
    protected override void OnDropDownControlBinding( CustomCombo.EventArgsBindDropDownControl e )
    {
      e.BindedControl = m_tree;
      m_tree.ImageList = m_imgList;
      RaiseFillTreeByData( e );
      
      // in case when we do data load on scroll message then 
      m_ctrlBinded = m_tree;
      m_bControlBinded = true;
    }
    protected override void OnDropDownSizeChanged()
    {
      base.OnDropDownSizeChanged();
      
      if( m_dropDownForm != null )
      {
        int iCountFull = m_tree.GetNodeCount( true );
        int iCountShort = m_tree.GetNodeCount( false );
        int iHeight = m_tree.ItemHeight;

        int totalHeight = DropDownHeigth;

        if( iCountShort * iHeight < DropDownHeigth && 
          iCountFull * iHeight < DropDownHeigth )
        {
          totalHeight = iCountFull * iHeight + 2;
        }

        m_dropDownForm.Size = new Size( ( DropDownWidth == 0 || ClientRectangle.Width > DropDownWidth ) ? ClientRectangle.Width : DropDownWidth , totalHeight );
      }
    
    }
    #endregion

    #region Class helper methods
    public    void UserRaiseFillData()
    {
      CustomCombo.EventArgsBindDropDownControl e = 
        new CustomCombo.EventArgsBindDropDownControl( this, m_dropDownForm, m_tree );

      OnDropDownControlBinding( e );
    }

    protected void RaiseFillTreeByData( CustomCombo.EventArgsBindDropDownControl e )
    {
      if( DataFill != null && m_bFillCalled == false )
      {
        EventArgsTreeDataFill ev = new EventArgsTreeDataFill( e );

        m_tree.BeginUpdate();
        DataFill( this, ev );
        m_tree.EndUpdate();

        m_bFillCalled = true;
      }
    }
    #endregion

    #region Scroll and Value Set Support
    protected override void OnPrevScrollItems()
    {
      if( m_tree.SelectedNode == null )
      {
        if( m_tree.Nodes.Count == 0 && m_bFillCalled == false )
        {
          CustomCombo.EventArgsBindDropDownControl e = new CustomCombo.EventArgsBindDropDownControl( this, m_dropDownForm, m_tree );
          OnDropDownControlBinding( e );
        }
        
        if( m_tree.Nodes.Count > 0 )
        {
          m_tree.SelectedNode = m_tree.Nodes[0];
          base.Value = m_tree.SelectedNode.Text;
        }
      }
      else
      {
        m_tree.SelectedNode = FindPrevNode( m_tree );
        if( m_tree.SelectedNode != null )
        {
          base.Value = m_tree.SelectedNode.Text;
        }
      }
    }

    protected override void OnNextScrollItems()
    {
      if( m_tree.SelectedNode == null )
      {
        if( m_tree.Nodes.Count == 0 && m_bFillCalled == false )
        {
          CustomCombo.EventArgsBindDropDownControl e = new CustomCombo.EventArgsBindDropDownControl( this, m_dropDownForm, m_tree );
          OnDropDownControlBinding( e );
        }

        if( m_tree.Nodes.Count > 0 )
        {
          m_tree.SelectedNode = m_tree.Nodes[0];
          base.Value = m_tree.SelectedNode.Text;
        }
      }
      else
      {
        m_tree.SelectedNode = FindNextNode( m_tree );
        
        if( m_tree.SelectedNode != null )
        {
          base.Value = m_tree.SelectedNode.Text;
        }
      }
    }
    protected override void OnValueChanged()
    {
      // TODO: find item by value name
      base.OnValueChanged();
    }
    protected override void OnSetFindItem( char ch )
    {
      string find = "" + ch;
      
      TreeNode node = null;
      TreeNode curr = m_tree.SelectedNode;
      
      IList array = new ArrayList();
      FindNodesByTextEx( find, array );

      if( curr != null )
      {
        int index = array.IndexOf( curr );
        
        if( index >= 0 && index < array.Count-1 )
        {
          node = (TreeNode)array[ index+1 ];
        }
        else if( array.Count > 0 )
        {
          node = (TreeNode)array[0];
        }
        else
        {
          node = curr;
        }
      }
      else
      {
        if( array.Count > 0 ) node = (TreeNode)array[0];
      }
      
      m_tree.SelectedNode = node;
      Refresh();
    }

    protected virtual  void OnCloseDropDownHandler( object sender, EventArgsCloseDropDown e )
    {
      if( m_tree.SelectedNode != null )
      {
        base.Value = m_tree.SelectedNode.Text;
      }
    }
    #endregion
    
    #region Tree Works Helper methods
    public static TreeNode FindNextNode( TreeView tree )
    {
      if( tree != null && tree.Nodes.Count > 0 )
      {
        TreeNode node1 = tree.SelectedNode;
        TreeNode backup = tree.SelectedNode;

        if( node1 == null ) return null;
      
        if( node1.Nodes.Count > 0 ) // if we have child the show it first
        {
          return node1.Nodes[0];
        }
        else
        {
          TreeNode node2 = node1;
          
          while( node2 != null )
          {
            if( node2.Parent == null ) // if we on the top of tree
            {
              if( node2.Index < tree.Nodes.Count-1 ) // check can we select next node or not
              {
                return tree.Nodes[ node2.Index + 1 ];
              }
              else // we on the last node in tree
              {
                // if we on last child of tree node
                if( node2 != backup ) return backup;
                
                return node2; 
              }
            }
            else // if we have a parent node
            {
              node1 = node2.Parent;
              
              if( node2.Index < node1.Nodes.Count-1 ) // can we select next node
              {
                return node1.Nodes[ node2.Index + 1 ];
              }
              else // go to the parent
              {
                node2 = node1;
              }
            }
          }
        }
      }

      return null;
    }

    public static TreeNode FindPrevNode( TreeView tree )
    {
      if( tree != null && tree.Nodes.Count > 0 )
      {
        TreeNode node1 = tree.SelectedNode;
        TreeNode backup = tree.SelectedNode;

        if( node1 == null ) return null; // if no selected node in tree

        if( node1.Parent == null ) 
        {
          // if we not on first node
          if( node1.Index > 0 && node1.Index < tree.Nodes.Count )
          {
            TreeNode node2 = tree.Nodes[ node1.Index - 1 ];
            node1 = node2;
            
            // find last child of new selected node
            if( node2.Nodes.Count > 0 )
            {
              do
              {
                node1 = node1.Nodes[ node2.Nodes.Count - 1 ];
              }
              while( node1.Nodes.Count != 0  );
            }

            return node1;
          }
          else
          {
            return node1;
          }
        }
        else
        {
          // if we are not a first child of parent
          if( node1.Index > 0 && node1.Index < node1.Parent.Nodes.Count )
          {
            TreeNode node2 = node1.Parent.Nodes[ node1.Index - 1 ];
            node1 = node2;
            
            // find last child of new selected node
            if( node2.Nodes.Count > 0 )
            {
              do
              {
                node1 = node1.Nodes[ node2.Nodes.Count - 1 ];
              }
              while( node1.Nodes.Count != 0  );
            }

            return node1;
          }
          else // first child of parent
          {
            return node1.Parent;
          }
        }
      }

      return null;
    }

    public static TreeNode FindNodeByText( TreeNodeCollection nodes, string value )
    {
      TreeNode found = null;

      foreach( TreeNode node in nodes )
      {
        if( node.Text == value )
          return node;
        
        if ( node.Nodes.Count > 0 && ( ( found = FindNodeByText( node.Nodes, value ) ) != null ) ) 
          return found;
      }
      
      return null;
    }

    public static void FindNodesByTextEx( TreeNodeCollection nodes, IList array, string value )
    {
      foreach( TreeNode node in nodes )
      {
        if( node.Text.ToLower().IndexOf( value ) == 0 )
        { 
          array.Add( node );
        }

        if( node.Nodes.Count > 0 )
        {
          FindNodesByTextEx( node.Nodes, array, value );
        }
      }
    }

    public static TreeNode FindNodeByTag( TreeNodeCollection nodes, object value )
    {
      TreeNode found = null;
      
      foreach( TreeNode node in nodes )
      {
        if ( node.Tag != null && node.Tag.Equals(value) )
          return node;
        
        if ( node.Nodes.Count > 0 && ( ( found = FindNodeByTag( node.Nodes, value ) ) != null ) ) 
          return found;
      }

      return null;
    }

    
    public void FindNodesByTextEx( string value, IList array )
    {
      FindNodesByTextEx( m_tree.Nodes, array, value.ToLower() );
    }

    public TreeNode FindNodeByText( string value )
    {
      return FindNodeByText( m_tree.Nodes, value );
    }
    
    public TreeNode FindNodeByTag( object value )
    {
      return FindNodeByTag( m_tree.Nodes, value );
    }
    
    public bool     SelectNodeByTag( object value )
    {
      m_tree.SelectedNode = FindNodeByTag( value );
      
      return (m_tree.SelectedNode != null);
    }
    #endregion

    #region Custom Draw
    protected virtual  void OnItemSizeCalculate( object sender, CustomCombo.EventArgsEditCustomSize e )
    {
      if( m_imgList != null )
      {
        int iWidth = m_imgList.ImageSize.Width + 2;
        e.xPos  += iWidth;
        e.Width -= iWidth;
      }
    }

    protected override void OnPaintCustomData(System.Windows.Forms.PaintEventArgs pevent)
    {
      Graphics g = pevent.Graphics;
      Rectangle rc = this.ClientRectangle; //pevent.ClipRectangle;

      if( m_tree.SelectedNode != null && m_imgList != null )
      {
        Rectangle rcOut = new Rectangle( rc.X + 2, rc.Y+2, m_imgList.ImageSize.Width, rc.Height - 4 );
        int index = m_tree.SelectedNode.ImageIndex;
        
        if( m_imgList.Images.Count > index && m_imgList.Images.Count > 0 )
        {
          if( index < 0 ) index = 0;
          Image img = m_imgList.Images[ index ];

          if( Enabled )
          {
            g.DrawImage( img, rcOut );
          }
          else
          {
            ControlPaint.DrawImageDisabled( g,img, rcOut.X, rcOut.Y, SystemColors.Control );
          }
        }
      }
    }
    #endregion

    #region Event Handlers
    public override void Refresh()
    {
      OnTreeItemChanged( this, null );
      base.Refresh();
    }

    protected virtual void OnTreeItemChanged( object sender, TreeViewEventArgs e )
    {
      if( m_tree.SelectedNode != null )
      {
        base.Value = m_tree.SelectedNode.Text;
      }
      else
      {
        base.Value = "";
      }
    }

    protected virtual void OnDropDownClicked( object sender, EventArgs e )
    {
      if( DropDownClicked != null )
      {
        DropDownClicked( sender, e );
      }

      if( m_bCloseOnClick )
      {
        base.DroppedDown = false;
      }
    }
    #endregion
  }
}
