using System.Collections.Generic;
using System.ComponentModel;

namespace TreeViewWithCheckBoxes
{
    public class TreeViewNode : INotifyPropertyChanged
    {
        #region Data

        bool? _isChecked = false;
        TreeViewNode _parent;

        #endregion // Data

        #region CreateFoos


        public TreeViewNode(string name, TreeViewNode parent)
        {
            this.Name = name;
            this.Children = new List<TreeViewNode>();
            this._parent = parent;
            this.IsNodeEnabled = true;
        }

        public void Initialize()
        {
            foreach (TreeViewNode child in this.Children)
            {
                child._parent = this;
                child.Initialize();
            }
        }

        #endregion // CreateFoos

        #region Properties

        public List<TreeViewNode> Children { get; private set; }

        public bool IsInitiallySelected { get; private set; }
        public bool IsNodeExpanded { get; set; }
        public bool IsNodeEnabled { get; set; }


        public string Name { get; private set; }
        public object Tag { get; set; }

        #region IsChecked

        /// <summary>
        /// Gets/sets the state of the associated UI toggle (ex. CheckBox).
        /// The return value is calculated based on the check state of all
        /// child FooViewModels.  Setting this property to true or false
        /// will set all children to the same check state, and setting it 
        /// to any value will cause the parent to verify its check state.
        /// </summary>
        public bool? IsChecked
        {
            get { return _isChecked; }
            set { this.SetIsChecked(value, true, true); }
        }

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked)
                return;

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue)
                this.Children.ForEach(c => c.SetIsChecked(_isChecked, true, false));

            if (updateParent && _parent != null)
                _parent.VerifyCheckState();

            this.OnPropertyChanged("IsChecked");
        }

        void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < this.Children.Count; ++i)
            {
                bool? current = this.Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = true;
                    break;
                }

            }
            this.SetIsChecked(state, false, true);
        }

        #endregion // IsChecked

        #endregion // Properties

        #region INotifyPropertyChanged Members

        void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}