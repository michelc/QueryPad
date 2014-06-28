using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace QueryPad
{
    // Ref http://codepaste.net/deke7q
    // <=> http://www.tech.windowsapplication1.com/content/sortable-binding-list-custom-data-objects
    public class SortableBindingList<T> : BindingList<T>
    {
        // constructor
        public SortableBindingList(List<T> list) : base(list) { }

        // fields
        private bool m_IsSorted;
        private ListSortDirection m_SortDirection;
        private PropertyDescriptor m_SortProperty;

        // properties
        protected override ListSortDirection SortDirectionCore { get { return m_SortDirection; } }
        protected override PropertyDescriptor SortPropertyCore { get { return m_SortProperty; } }
        protected override bool IsSortedCore { get { return m_IsSorted; } }
        protected override bool SupportsSortingCore { get { return true; } }

        // methods
        protected override void RemoveSortCore() { m_IsSorted = false; }
        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            if (prop.PropertyType.GetInterface("IComparable") == null) return;
            var _List = this.Items as List<T>;
            if (_List == null)
            {
                m_IsSorted = false;
            }
            else
            {
                var _Comparer = new PropertyComparer(prop.Name, direction);
                _List.Sort(_Comparer);
                m_IsSorted = true;
                m_SortDirection = direction;
                m_SortProperty = prop;
            }
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        // sub class
        public class PropertyComparer : IComparer<T>
        {
            // properties
            private PropertyInfo PropInfo { get; set; }
            private ListSortDirection Direction { get; set; }

            // methods
            public PropertyComparer(string propName, ListSortDirection direction)
            {
                this.PropInfo = typeof(T).GetProperty(propName);
                this.Direction = direction;
            }
            public int Compare(T x, T y)
            {
                var _X = PropInfo.GetValue(x, null);
                var _Y = PropInfo.GetValue(y, null);
                if (Direction == ListSortDirection.Ascending)
                    return Comparer.Default.Compare(_X, _Y);
                else
                    return Comparer.Default.Compare(_Y, _X);
            }
        }
    }
}
