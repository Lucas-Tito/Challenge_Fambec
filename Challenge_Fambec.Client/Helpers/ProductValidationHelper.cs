using Challenge_Fambec.Shared.Models.Entities;
using Challenge_Fambec.Shared.Models.Enums;

namespace Challenge_Fambec.Client.Helpers
{
    /// <summary>
    /// Helper class for product validation logic
    /// </summary>
    public static class ProductValidationHelper
    {
        /// <summary>
        /// Validates a product and returns validation errors
        /// </summary>
        /// <param name="product">The product to validate</param>
        /// <param name="isCreating">True if creating new product, false if editing</param>
        /// <param name="tipoItemSelected">True if TipoItem was manually selected (only for creating)</param>
        /// <returns>Dictionary of field names and error messages</returns>
        public static Dictionary<string, string> ValidateProduct(Product product, bool isCreating = true, bool tipoItemSelected = true)
        {
            var validationErrors = new Dictionary<string, string>();

            // Required field validations for creation
            if (isCreating)
            {
                if (string.IsNullOrWhiteSpace(product.CodItem))
                {
                    validationErrors["CodItem"] = "Item Code is required.";
                }
                else if (product.CodItem.Length > 60)
                {
                    validationErrors["CodItem"] = "Item Code must not exceed 60 characters.";
                }

                if (!tipoItemSelected)
                {
                    validationErrors["TipoItem"] = "Item Type is required.";
                }
            }

            // Common required field validations
            if (string.IsNullOrWhiteSpace(product.DescrItem))
            {
                validationErrors["DescrItem"] = "Description is required.";
            }

            if (string.IsNullOrWhiteSpace(product.UnidInv))
            {
                validationErrors["UnidInv"] = "Inventory Unit is required.";
            }

            // Conditional validation: If item type is Services, Service List Code is required
            if (product.TipoItem == TipoItem.Servicos)
            {
                if (string.IsNullOrWhiteSpace(product.CodLst))
                {
                    validationErrors["CodLst"] = "Service List Code is required when Item Type is Services.";
                }
            }

            // Optional field length validations
            if (!string.IsNullOrWhiteSpace(product.CodAntItem) && product.CodAntItem.Length > 60)
            {
                validationErrors["CodAntItem"] = "Previous Item Code must not exceed 60 characters.";
            }

            if (!string.IsNullOrWhiteSpace(product.CodNcm) && product.CodNcm.Length != 8)
            {
                validationErrors["CodNcm"] = "NCM Code must be exactly 8 characters.";
            }

            if (!string.IsNullOrWhiteSpace(product.ExIpi) && product.ExIpi.Length > 3)
            {
                validationErrors["ExIpi"] = "IPI Exception must not exceed 3 characters.";
            }

            if (!string.IsNullOrWhiteSpace(product.CodGen) && product.CodGen.Length > 2)
            {
                validationErrors["CodGen"] = "Genre Code must not exceed 2 characters.";
            }

            if (!string.IsNullOrWhiteSpace(product.CodGen) && !product.CodGen.All(char.IsDigit))
            {
                validationErrors["CodGen"] = "Genre Code must contain only numbers.";
            }

            if (!string.IsNullOrWhiteSpace(product.CodLst) && product.CodLst.Length > 5)
            {
                validationErrors["CodLst"] = "Service List Code must not exceed 5 characters.";
            }

            if (!string.IsNullOrWhiteSpace(product.CodCest))
            {
                if (product.CodCest.Length != 7)
                {
                    validationErrors["CodCest"] = "CEST Code must be exactly 7 digits.";
                }
                else if (!product.CodCest.All(char.IsDigit))
                {
                    validationErrors["CodCest"] = "CEST Code must contain only numbers.";
                }
            }

            if (product.AliqIcms.HasValue && (product.AliqIcms < 0 || product.AliqIcms > 100))
            {
                validationErrors["AliqIcms"] = "ICMS Rate must be between 0 and 100.";
            }

            return validationErrors;
        }

        /// <summary>
        /// Checks if a product has all required fields filled for creation
        /// </summary>
        /// <param name="product">The product to check</param>
        /// <param name="tipoItemSelected">True if TipoItem was manually selected</param>
        /// <returns>True if form is valid for creation</returns>
        public static bool IsValidForCreation(Product product, bool tipoItemSelected)
        {
            return !string.IsNullOrWhiteSpace(product.CodItem) &&
                   !string.IsNullOrWhiteSpace(product.DescrItem) &&
                   !string.IsNullOrWhiteSpace(product.UnidInv) &&
                   tipoItemSelected;
        }

        /// <summary>
        /// Checks if a product has all required fields filled for editing
        /// </summary>
        /// <param name="product">The product to check</param>
        /// <returns>True if form is valid for editing</returns>
        public static bool IsValidForEditing(Product product)
        {
            return !string.IsNullOrWhiteSpace(product.DescrItem) &&
                   !string.IsNullOrWhiteSpace(product.UnidInv) &&
                   Enum.IsDefined(typeof(TipoItem), product.TipoItem);
        }

        /// <summary>
        /// Checks if Service List Code should be required based on item type
        /// </summary>
        /// <param name="tipoItem">The item type</param>
        /// <returns>True if Service List Code is required</returns>
        public static bool IsServiceListCodeRequired(TipoItem tipoItem)
        {
            return tipoItem == TipoItem.Servicos;
        }

        /// <summary>
        /// Gets display name for TipoItem enum values
        /// </summary>
        /// <param name="tipo">The TipoItem enum value</param>
        /// <returns>The display name</returns>
        public static string GetTipoItemDisplayName(TipoItem tipo)
        {
            return tipo switch
            {
                TipoItem.MercadoriaParaRevenda => "Merchandise for Resale",
                TipoItem.MateriaPrima => "Raw Material",
                TipoItem.Embalagem => "Packaging",
                TipoItem.ProdutoEmProcesso => "Work in Process",
                TipoItem.ProdutoAcabado => "Finished Product",
                TipoItem.Subproduto => "By-product",
                TipoItem.ProdutoIntermediario => "Intermediate Product",
                TipoItem.MaterialDeUsoEConsumo => "Supplies and Consumables",
                TipoItem.AtivoImobilizado => "Fixed Assets",
                TipoItem.Servicos => "Services",
                TipoItem.OutrosInsumos => "Other Inputs",
                TipoItem.Outras => "Others",
                _ => tipo.ToString()
            };
        }
    }
}
