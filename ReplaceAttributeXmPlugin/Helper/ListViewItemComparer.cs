using System.Collections;
using System.Windows.Forms;

namespace ReplaceAttributeXmPlugin.Helper
{
    internal class ListViewItemComparer : IComparer
    {
        private readonly int _col;

        private readonly SortOrder _innerOrder;

        public ListViewItemComparer(int column, SortOrder order)
        {
            this._col = column;
            this._innerOrder = order;
        }

        public int Compare(object x, object y)
        {
            return this.Compare((ListViewItem)x, (ListViewItem)y);
        }

        private int Compare(ListViewItem x, ListViewItem y)
        {
            return this._innerOrder == SortOrder.Ascending ? string.CompareOrdinal(x.SubItems[this._col].Text, y.SubItems[this._col].Text) : string.CompareOrdinal(y.SubItems[this._col].Text, x.SubItems[this._col].Text);
        }


    }
}
