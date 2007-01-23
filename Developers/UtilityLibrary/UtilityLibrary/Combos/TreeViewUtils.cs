using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace UtilityLibrary.General
{
  public class TreeViewUtils
  {
    /// <summary>
    /// Convert Tree Node structure to plain array
    /// </summary>
    /// <param name="tree">TreeView Control</param>
    /// <returns>array of TreeView Items</returns>
    public static TreeNode[] ToPlainArray( TreeView tree )
    {
      return ToPlainArray( tree.Nodes );
    }

    public static TreeNode[] ToPlainArray( TreeNodeCollection nodes )
    {
      TreeNode[] arrayOut;

      if( nodes.Count > 0 )
      {
        ArrayList array = new ArrayList();

        TreeNode  next = null, start = nodes[0];
        array.Add( start );

        while( null != ( next = FindNextNode( nodes, start ) ) )
        {
          if( next == start ) break;
          array.Add( start = next );
        }

        arrayOut = new TreeNode[ array.Count ];
        array.CopyTo( arrayOut, 0 );

        return arrayOut;
      }

      return null;
    }
    /// <summary>
    /// Find Next node in Tree from SelectedNode
    /// </summary>
    /// <param name="tree"></param>
    /// <returns></returns>
    public static TreeNode FindNextNode( TreeView tree )
    {
      return FindNextNode( tree.Nodes, tree.SelectedNode );
    }
    public static TreeNode FindNextNode( TreeNodeCollection nodes, TreeNode selected )
    {
      if( nodes != null && nodes.Count > 0 )
      {
        TreeNode node1 = ( selected == null ) ? nodes[0] : selected;
        TreeNode backup = node1;

        if( node1.Nodes.Count > 0 ) // if we have child the show it first
        {
          return node1.Nodes[0];
        }
        else
        {
          TreeNode node2 = node1;
          
          while( node2 != null )
          {
            if( node2.Parent == null || nodes.Contains( node2 ) == true ) // if we on the top of tree
            {
              if( node2.Index < nodes.Count-1 ) // check can we select next node or not
              {
                return nodes[ node2.Index + 1 ];
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

    /// <summary>
    /// Find Previous node from Selected node
    /// </summary>
    /// <param name="tree"></param>
    /// <returns></returns>
    public static TreeNode FindPrevNode( TreeView tree )
    {
      return FindPrevNode( tree.Nodes, tree.SelectedNode );
    }

    public static TreeNode FindPrevNode( TreeNodeCollection nodes, TreeNode selected )
    {
      if( nodes != null && nodes.Count > 0 )
      {
        TreeNode node1 = ( selected == null ) ? nodes[0] : selected;
        TreeNode backup = node1;

        if( node1.Parent == null || nodes.Contains( node1 ) == true ) 
        {
          // if we not on first node
          if( node1.Index > 0 && node1.Index < nodes.Count )
          {
            TreeNode node2 = nodes[ node1.Index - 1 ];
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

    /// <summary>
    /// Find Node in Tree by name
    /// </summary>
    /// <param name="tree"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TreeNode FindNodeByText( TreeView tree, string value )
    {
      return FindNodeByText( ToPlainArray( tree ), value );
    }
    
    public static TreeNode FindNodeByText( TreeNode[] array, string value )
    {
      if( array != null )
      {
        foreach( TreeNode node in array )
        {
          if( node.Text == value ) return node;
        }
      }
      
      return null;
    }
    
    /// <summary>
    /// Find node in tree by Regular Expression
    /// </summary>
    /// <param name="tree"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TreeNode FindNodeByTextEx( TreeView tree, string value )
    {
      return FindNodeByTextEx( ToPlainArray( tree ), value );
    }

    public static TreeNode FindNodeByTextEx( TreeNode[] array, string value )
    {
      if( array != null )
      {
        Regex reg = new Regex( value, RegexOptions.IgnoreCase|RegexOptions.Compiled );

        foreach( TreeNode node in array )
        {
          if( reg.Match( node.Text ).Success == true )
            return node;
        }
      }
      
      return null;
    }

    public static TreeNode FindNodeByTextEx( TreeNode[] array, string value, int pos )
    {
      if( array != null )
      {
        Regex reg = new Regex( value, RegexOptions.IgnoreCase|RegexOptions.Compiled );

        int iCount = 0;

        foreach( TreeNode node in array )
        {
          if( iCount++ < pos ) continue;
          if( reg.Match( node.Text ).Success == true )
          {
            return node;
          }
        }
      }
      
      return null;
    }

    /// <summary>
    /// Find Node by Tag property
    /// </summary>
    /// <param name="tree"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TreeNode FindNodeByTag( TreeView tree, object value )
    {
      return FindNodeByTag( ToPlainArray( tree ), value );    
    }

    public static TreeNode FindNodeByTag( TreeNode[] array, object value )
    {
      if( array != null )
      {
        foreach( TreeNode node in array )
        {
          if (node != null && node.Tag != null) 
            if( node.Tag.Equals( value ) == true )
              return node;
        }
      }

      return null;
    }

    public static TreeNode[] FindNodesByTag( TreeNode[] array, object value )
    {
      if( array != null )
      {
        TreeNode[] list = new TreeNode[ array.GetLength(0) ];
        int icount = 0;

        foreach( TreeNode node in array )
        {
          if (node != null && node.Tag != null) 
            if( node.Tag.Equals( value ) == true )
              list[ icount++ ] = node;
        }

        TreeNode[] res = new TreeNode[ icount ];    
        for( int i = 0; i < icount; i++ )
        {
          res[i] = list[i];
        }
        return res;
      }
      else
        return null;
    }

    public static TreeNode FindNodeByTag( TreeNode[] array, object value, int startPos )
    {
      if( array != null )
      {
        int iCount = 0;

        foreach( TreeNode node in array )
        {
          if( iCount++ < startPos ) continue;

          if( node.Tag != null )
          {
            if( node.Tag.Equals( value ) == true )
            {
              return node;
            }
          }
        }
      }

      return null;
    }

    /// <summary>
    /// Build TreeNodes caches for fast find and update
    /// </summary>
    public static IDictionary BuildTagsCache( TreeNode[] array )
    {
      IDictionary dict = new SortedList();

      foreach( TreeNode node in array )
      {
        int indexTag = ( node.Tag == null ) ? -1 : node.Tag.GetHashCode();

        if( dict.Contains( indexTag ) == false )
        {
          dict[ indexTag ] = new ArrayList();
        }

        ((IList)dict[ indexTag ]).Add( node );
      }

      return dict;
    }

    public static IDictionary BuildTextCache( TreeNode[] array )
    {
      IDictionary dict = new SortedList();

      foreach( TreeNode node in array )
      {
        if( dict.Contains( node.Text ) == false )
        {
          dict[ node.Text ] = new ArrayList();
        }

        ((IList)dict[ node.Text ]).Add( node );
      }

      return dict;
    }
  }
}
