using System.ComponentModel;

namespace XamlDemo.Model
{
    /// <summary>
    /// 一个实现了 INotifyPropertyChanged 接口的实体类
    /// </summary>
    public class Employee : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        private int _age;
        public int Age
        {
            get { return _age; }
            set 
            {
                _age = value;
                RaisePropertyChanged("Age");
            }
        }

        private bool _isMale;
        public bool IsMale
        {
            get { return _isMale; }
            set
            {
                _isMale = value;
                RaisePropertyChanged("IsMale");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
