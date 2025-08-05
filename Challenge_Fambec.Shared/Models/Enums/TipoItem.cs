namespace Challenge_Fambec.Shared.Models.Enums;

/// <summary>
/// Enum representing the different item types according to EFD layout standards.
/// Comments reference the original EFD layout codes.
/// </summary>
public enum TipoItem
{
    MercadoriaParaRevenda = 0,    // 00 - Merchandise for resale
    MateriaPrima = 1,             // 01 - Raw material  
    Embalagem = 2,                // 02 - Packaging
    ProdutoEmProcesso = 3,        // 03 - Work in process
    ProdutoAcabado = 4,           // 04 - Finished product
    Subproduto = 5,               // 05 - By-product
    ProdutoIntermediario = 6,     // 06 - Intermediate product
    MaterialDeUsoEConsumo = 7,    // 07 - Supplies and consumables
    AtivoImobilizado = 8,         // 08 - Fixed assets
    Servicos = 9,                 // 09 - Services
    OutrosInsumos = 10,           // 10 - Other inputs
    Outras = 99                   // 99 - Others
}
