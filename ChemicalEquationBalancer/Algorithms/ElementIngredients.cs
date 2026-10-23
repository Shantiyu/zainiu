using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemicalEquationBalancer.Algorithms
{
    public class ElementIngredients
    {
        public Dictionary<string, int> elements = new Dictionary<string, int>();
        public int electricCharge = 0;
        public int this[string elementName]
        {
            get
            {
                if (elements.ContainsKey(elementName))
                    return elements[elementName];
                else return 0;
            }
            set 
            {
                if (elements.ContainsKey(elementName))
                    elements[elementName] = value;
                else
                    elements.Add(elementName, value);
            }
        }

        public ElementIngredients AddBy(ElementIngredients elements2)
        {
            ElementIngredients e = new ElementIngredients();
            foreach (var i in elements.Keys) 
            {
                e[i] = e[i] + this[i];
            }
            foreach (var i in elements2.elements.Keys)
            {
                e[i] = e[i] + elements2[i];
            }
            e.electricCharge = electricCharge + elements2.electricCharge;
            return e;
        }
        public void SelfAddBy(ElementIngredients elements2)
        {
            foreach (var i in elements2.elements.Keys)
            {
                this[i] = this[i] + elements2[i];
            }
            electricCharge += elements2.electricCharge;
        }
        public bool IsEqual(ElementIngredients elements2)
        {
            if (elements2.electricCharge != electricCharge) return false;
            foreach (var i in elements.Keys)
            {
                if (this[i] != elements2[i]) return false;
            }
            foreach (var i in elements2.elements.Keys)
            {
                if (this[i] != elements2[i]) return false;
            }
            return true;
        }
        public ElementIngredients MultiplyBy(int mul)
        {
            ElementIngredients e = new ElementIngredients();
            foreach (var i in elements.Keys)
            {
                e[i] = this[i] * mul;
            }
            e.electricCharge = electricCharge * mul;
            return e;
        }
        public void SelfAddIngredient(string elementName, int value)
        {
            this[elementName] = this[elementName] + value;
        }

        public double CalculateExactRelativeAtomicMass()
        {
            double m = 0;
            foreach (var r in elements.Keys)
            {
                m += RelativeAtomicMass.GetExactMassOf(r) * elements[r];
            }
            return m;
        }
        public double CalculateSimpleRelativeAtomicMass()
        {
            double m = 0;
            foreach (var r in elements.Keys)
            {
                m += RelativeAtomicMass.GetSimpleMassOf(r) * elements[r];
            }
            return m;
        }
    }
}
