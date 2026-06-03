using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

var email = new EmailAddressAttribute();

Console.WriteLine(email.IsValid("someone@somewhere.com"));         //true
Console.WriteLine(email.IsValid("someone@somewhere.co.uk"));       //true
Console.WriteLine(email.IsValid("someone+tag@somewhere.net"));     //true
Console.WriteLine(email.IsValid("futureTLD@somewhere.fooo"));      //true

Console.WriteLine(email.IsValid("fdsa"));                          //false
Console.WriteLine(email.IsValid("fdsa@"));                         //false
Console.WriteLine(email.IsValid("fdsa@fdsa"));                     //false
Console.WriteLine(email.IsValid("fdsa@fdsa."));                    //false


string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
var regex = new Regex(pattern, RegexOptions.IgnoreCase);

Console.WriteLine(regex.IsMatch("fdsa@fdsa"));
Console.WriteLine(regex.IsMatch("fdsa@fdsa."));
Console.WriteLine(regex.IsMatch("someone@somewhere.co.uk"));
Console.WriteLine(regex.IsMatch("someone+tag@somewhere.net"));
Console.WriteLine(regex.IsMatch("futureTLD@somewhere.fooo"));

