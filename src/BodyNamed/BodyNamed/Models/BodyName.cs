using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Newtonsoft.Json;

namespace BodyNamed.Models
{

    public class BodyNames : ObservableCollection<BodyName>
    {

    }
    public class BodyName : INotifyPropertyChanged
    {
        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private Gender gender;

        public Gender Gender
        {
            get { return gender; }
            set
            {
                gender = value;
                OnPropertyChanged(nameof(Gender));
            }
        }

        private double? chance;

        public double? Chance
        {
            get { return chance; }
            set
            {
                chance = value;
                OnPropertyChanged(nameof(Chance));
            }
        }

        private string introduction;

        public string Introduction
        {
            get { return introduction; }
            set
            {
                introduction = value;
                OnPropertyChanged(nameof(Introduction));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        [JsonIgnore]
        public int Min { get; set; }
        [JsonIgnore]
        public int Max { get; set; }
    }

    public enum Gender : int
    {
        Male,
        Female
    }
}
