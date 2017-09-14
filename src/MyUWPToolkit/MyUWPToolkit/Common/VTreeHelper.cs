using System;
using System.Collections.Generic;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace MyUWPToolkit.Common
{
    static class VTreeHelper
    {
        /// <summary>
        /// Returns the first child visual object of the specified type within a specified parent.
        /// </summary>
        /// <param name="reference">The parent visual, referenced as a <see cref="DependencyObject"/>.</param>
        /// <param name="type">The <see cref="Type"/> of the children element to search for.</param>
        /// <returns>The visual object of the specified type.</returns>
        public static DependencyObject GetChildOfType(DependencyObject reference, Type type)
        {
            DependencyObject el = null;
            if (VisualTreeHelper.GetChildrenCount(reference) > 0)
            {
                el = VisualTreeHelper.GetChild(reference, 0);
                while (el != null)
                {
                    if (type.IsAssignableFrom(el.GetType()))
                    {
                        return el;
                    }
                    if (VisualTreeHelper.GetChildrenCount(el) > 0)
                    {
                        el = VisualTreeHelper.GetChild(el, 0);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Returns all children visual objects of the specified type within a specified parent.
        /// </summary>
        /// <param name="reference">The parent visual, referenced as a <see cref="DependencyObject"/>.</param>
        /// <param name="type">The <see cref="Type"/> of the children element to search for.</param>
        /// <param name="list">The <see cref="IList{DependencyObject}"/> object to fill with found objects.</param>
        public static void GetChildrenOfType(DependencyObject reference, Type type, ref IList<DependencyObject> list)
        {
            DependencyObject el = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(reference);
            for (int i = 0; i < childrenCount; i++)
            {
                el = VisualTreeHelper.GetChild(reference, i);
                if (el != null)
                {
                    if (type.IsAssignableFrom(el.GetType()))
                    {
                        list.Add(el);
                        continue;
                    }
                    if (VisualTreeHelper.GetChildrenCount(el) > 0)
                    {
                        GetChildrenOfType(el, type, ref list);
                    }
                }
            }
        }
        /// <summary>
        /// Returns a <see cref="DependencyObject"/> value that represents the parent 
        /// of the visual object of the specified type. Method looks up the visual tree until
        /// the desired parent element is found or parent element is null.
        /// </summary>
        /// <param name="reference">The visual whose parent is returned.</param>
        /// <param name="type">The <see cref="Type"/> of the parent element to search for.</param>
        /// <returns>The parent of the visual.</returns>
        public static DependencyObject GetParentOfType(DependencyObject reference, Type type)
        {
            return GetParentOfType(reference, type, null, false);
        }
        /// <summary>
        /// Returns a <see cref="DependencyObject"/> value that represents the parent 
        /// of the visual object of the specified type. Method looks up the visual tree until
        /// the desired parent element is found or parent element is equal to the specified endObject.
        /// </summary>
        /// <param name="reference">The visual whose parent is returned.</param>
        /// <param name="type">The <see cref="Type"/> of the parent element to search for.</param>
        /// <param name="endObject">The visual representing the end point of search.</param>
        /// <returns>The parent of the visual.</returns>
        public static DependencyObject GetParentOfType(DependencyObject reference, Type type, DependencyObject endObject)
        {
            return GetParentOfType(reference, type, endObject, false);
        }

        /// <summary>
        /// Returns a <see cref="DependencyObject"/> value that represents the parent 
        /// of the visual object of the specified type. Method looks up the visual tree until
        /// the desired parent element is found or parent element is null.
        /// </summary>
        /// <param name="reference">The visual whose parent is returned.</param>
        /// <param name="type">The <see cref="Type"/> of the parent element to search for.</param>
        /// <param name="lookOutsideVisualTree">Specifies whether the search should go on outside the VisualTree.</param>
        /// <returns>The parent of the visual.</returns>
        public static DependencyObject GetParentOfType(DependencyObject reference, Type type, bool lookOutsideVisualTree)
        {
            return GetParentOfType(reference, type, null, lookOutsideVisualTree);
        }

        /// <summary>
        /// Returns a <see cref="DependencyObject"/> value that represents the parent 
        /// of the visual object of the specified type. Method looks up the visual tree until
        /// the desired parent element is found or parent element is equal to the specified endObject.
        /// </summary>
        /// <param name="reference">The visual whose parent is returned.</param>
        /// <param name="type">The <see cref="Type"/> of the parent element to search for.</param>
        /// <param name="endObject">The visual representing the end point of search.</param>
        /// <param name="lookOutsideVisualTree">Specifies whether the search should go on outside the VisualTree.</param>
        /// <returns>The parent of the visual.</returns>
        public static DependencyObject GetParentOfType(DependencyObject reference, Type type, DependencyObject endObject, bool lookOutsideVisualTree)
        {
            if (reference == null)
                return null;
            while (true)
            {
                var parent = VisualTreeHelper.GetParent(reference);
                if (parent == null && lookOutsideVisualTree)
                {
                    var fel = reference as FrameworkElement;
                    if (fel != null)
                    {
                        parent = fel.Parent;
                    }
                }
                if (parent == null || parent == endObject)
                {
                    return null;
                }
                if (type.IsAssignableFrom(parent.GetType()))
                {
                    return parent;
                }
                reference = parent;
            }
        }

        /// <summary>
        /// Returns a <see cref="DependencyObject"/> value that represents the first parent 
        /// of the visual object which type is one of the specified tyles. Method looks up the visual tree until
        /// the desired parent element is found or parent element is equal to the specified endObject.
        /// </summary>
        /// <param name="reference">The visual whose parent is returned.</param>
        /// <param name="types">The <see cref="IList{Type}"/> list of types to search for.</param>
        /// <param name="endObject">The visual representing the end point of search.</param>
        /// <returns>The parent of the visual.</returns>
        public static DependencyObject GetFirstParent(DependencyObject reference, IList<Type> types, DependencyObject endObject)
        {
            if (reference == null)
                return null;
            while (true)
            {
                var parent = VisualTreeHelper.GetParent(reference);
                if (parent == null || parent == endObject)
                {
                    return null;
                }
                Type parentType = parent.GetType();
                foreach (Type type in types)
                {
                    if (type.IsAssignableFrom(parentType))
                    {
                        return parent;
                    }
                }
                reference = parent;
            }
        }

    }
}
