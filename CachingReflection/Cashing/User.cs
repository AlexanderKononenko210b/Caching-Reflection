using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Cashing
{
    /// <summary>
    /// Represents a model of the User class.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the second name.
        /// </summary>
        public string SecondName { get; set; }

        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets the additional info.
        /// </summary>
        public AdditionInfo AdditionInfo { get; set; }

        /// <summary>
        /// Get user instance as string.
        /// </summary>
        /// <returns>string performance user instance</returns>
        public override string ToString()
        {
            return
                $"FirstName = {this.FirstName}, SecondName = {this.SecondName}, Age = {this.Age}," +
                $" Phone = {AdditionInfo.Phone}, Fax = {AdditionInfo.Fax}";
        }
    }
}
