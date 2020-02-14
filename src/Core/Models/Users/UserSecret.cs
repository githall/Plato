using System;
using System.Data;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Abstractions;

namespace PlatoCore.Models.Users
{

    public class UserSecret : IDbModel<UserSecret>
    {

        #region "Public Properties"

        public int Id { get; set; }

        public int UserId { get; set; }

        public string Secret { get; set; }

        public int[] Salts { get; set; }

        public string SecurityStamp { get; set; }

        #endregion

        #region "Constructor"

        public UserSecret()
        {
            
        }

        public UserSecret(IDataReader reader)
        {
            PopulateModel(reader);
        }

        #endregion

        #region "Implementation"

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                this.Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("UserId"))
                this.UserId = Convert.ToInt32(dr["UserId"]);

            if (dr.ColumnIsNotNull("Secret"))
                this.Secret = Convert.ToString(dr["Secret"]);

            if (dr.ColumnIsNotNull("Salts"))
                this.Salts = Convert.ToString(dr["Salts"]).ToIntArray();

            if (dr.ColumnIsNotNull("SecurityStamp"))
                this.SecurityStamp = Convert.ToString(dr["SecurityStamp"]);

        }

        public void PopulateModel(Action<UserSecret> model)
        {
            model(this);
        }

        #endregion

    }

}
