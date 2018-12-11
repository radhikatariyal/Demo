using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Patient.Demographics.Common
{
    /// This was taken from https://lostechies.com/jimmybogard/2008/08/12/enumeration-classes/
    /// with some modifications from http://eliot-jones.com/2015/03/entity-framework-enum
    [Serializable]
    public abstract class Enumeration : IComparable
    {
        private string _displayName;
        private string _name;
        private int _index;

        protected Enumeration()
        {
        }

        protected Enumeration(string name, string displayName, int index = 0)
        {
            _displayName = displayName;
            _name = name;
            _index = index;
        }

        protected Enumeration(string name, int index = 0)
        {
            _name = name;
            _displayName = name;
            _index = index;
        }

        [JsonProperty]
        public string DisplayName => _displayName;

        [JsonProperty]
        public string Name
        {
            get { return _name; }

            // Entity Framework will only retrieve and set the  name.
            // Use this setter to find the corresponding value as defined in the static fields.
            protected set
            {
                _name = value;

                // Get the static fields on the inheriting type.
                foreach (var field in GetType().GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    // If the static field is an Enumeration type.
                    var enumeration = field.GetValue(this) as Enumeration;
                    if (enumeration == null)
                    {
                        continue;
                    }

                    // Set the value of this instance to the value of the corresponding static type.
                    if (string.Compare(enumeration.Name, value, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        _displayName = enumeration.DisplayName;
                        _index = enumeration.Index;
                        break;
                    }
                }
            }
        }

        [JsonProperty]
        public int Index => _index;

        public override string ToString()
        {
            return Name;
        }

        public static IEnumerable<T> GetAll<T>() where T : Enumeration, new()
        {
            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (var info in fields)
            {
                var instance = new T();
                var locatedValue = info.GetValue(instance) as T;

                if (locatedValue != null)
                {
                    yield return locatedValue;
                }
            }
        }

        public override bool Equals(object obj)
        {
            var otherValue = obj as Enumeration;

            if (otherValue == null)
            {
                return false;
            }

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = _displayName.Equals(otherValue.DisplayName);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode()
        {
            return _displayName.GetHashCode();
        }

        public static T FromName<T>(string name) where T : Enumeration, new()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            var matchingItem = parse<T, string>(name, "value", item => item.Name.ToLower() == name.ToLower());
            return matchingItem;
        }

        public static T FromDisplayName<T>(string displayName) where T : Enumeration, new()
        {
            var matchingItem = parse<T, string>(displayName, "display name", item => item.Name == displayName);
            return matchingItem;
        }

        private static T parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration, new()
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
            {
                var message = string.Format("'{0}' is not a valid {1} in {2}", value, description, typeof(T));
                throw new ApplicationException(message);
            }

            return matchingItem;
        }

        public int CompareTo(object other)
        {
            return DisplayName.CompareTo(((Enumeration)other).DisplayName);
        }

        public static bool operator ==(Enumeration a, Enumeration b)
        {
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            var typeMatches = a.GetType().Equals(b.GetType());
            var valueMatches = a.DisplayName!=null && a.DisplayName.Equals(b.DisplayName);

            return typeMatches && valueMatches;
        }

        public static bool operator !=(Enumeration a, Enumeration b)
        {
            return !(a == b);
        }
    }
}