using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Domain.Validators
{
    public static class StaticValidators
    {
        public static bool ValidateCNP(string cnp)
        {
            if (string.IsNullOrEmpty(cnp))
                return false;

            long cnpNumber;
            //check if the length is 13 and is numeric
            if (cnp.Length != 13 || !long.TryParse(cnp, out cnpNumber))
                return false;

            //split cnp into parts
            //GenderKey
            var gender = int.Parse((cnp.Substring(0, 1)));
            //check if the first number is between 1 and 8
            if (gender < 1 || gender > 8)
                return false;

            //Birth Year
            // ReSharper disable UnusedVariable
            var year = cnp.Substring(1, 2);
            // ReSharper restore UnusedVariable

            //Birth Month
            var month = int.Parse(cnp.Substring(3, 2));
            //check if month between 1 and 12
            if (month < 1 || month > 12)
                return false;

            //Birth Day
            var day = int.Parse(cnp.Substring(5, 2));
            //check if day between 1 and 31
            if (day < 1 || day > 31)
                return false;

            //Control Key
            var key = cnp.Substring(12, 1);
            //The constant compare number
            const string compareNumber = "279146358279";

            var sum = 0;
            //Multiply each position of the [compare number] with the same position of the [cnp number except Key] number then add them all
            for (var i = 0; i < 12; i++)
                sum += int.Parse(cnp[i].ToString(CultureInfo.InvariantCulture)) * int.Parse(compareNumber[i].ToString(CultureInfo.InvariantCulture));
            //Get the rest from dividing to 11. 
            sum = (sum % 11);
            //If the rest is 10 then transform the rest into 1.
            sum = sum == 10 ? 1 : sum;

            return (sum.ToString(CultureInfo.InvariantCulture) == key);
        }

        public static bool ValidateCIF(string cif)
        {
            if (string.IsNullOrWhiteSpace(cif))
                return false;

            // Maximum length is 10
            if (cif.Length > 10)
                return false;

            // Should only contain numbers
            var regex = new Regex("[^0-9]");
            var matches = regex.IsMatch(cif);

            if (matches)
                return false;

            // Using the key 753217532
            var key = "753217532".Reverse().ToList();
            // Invert the numbers from the CIF code
            var invertedCIF = cif.Reverse().ToList();

            var sum = 0;

            // First number in reversed sequence is the control key.
            // For each of the rest of the numbers in the reversed sequence, multiply with the number from the given key on the same position and add it to a sum
            for (var i = 1; i < invertedCIF.Count; i++)
                sum += int.Parse(invertedCIF[i].ToString()) * int.Parse((key[i - 1].ToString()));

            // Multiply the sum with 10 and get the rest of dividing by 11
            var check = (sum * 10) % 11;
            // if the rest is 10, make the rest 0
            check = check % 10;

            // if the checked number equals to the last digit of the CIF code (first of the reversed code)
            return check == int.Parse(invertedCIF[0].ToString());
        }
    }
}