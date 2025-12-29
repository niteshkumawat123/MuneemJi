using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MUNEEMJI.Models
{

    public class Expense
    {
        public int Id { get; set; }


        [StringLength(100)]
        public string Category { get; set; }

        public int CategoryId { get; set; }

        [StringLength(100)]
        public string ItemName { get; set; }

        [StringLength(50)]
        public string ItemHsnSac { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }


        [StringLength(20)]
        public string TaxType { get; set; } = "Tax Excluded";


        [StringLength(20)]
        public string TaxRate { get; set; } = "IGST@0.25%";


        public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string ExpenseNo { get; set; }

        [StringLength(100)]
        public string Party { get; set; }


        [StringLength(20)]
        public string PaymentType { get; set; } = "Cash";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }
        public string Description { get; set; }
        public bool Isroundoff { get; set; }
        public decimal roundoffvalue { get; set; }  
        public int PartyId { get; set; }  
        public string ImageUrl { get; set; }

        public int count { get; set; }

    }

    public class ExpenseViewModel
    {
        public Expense Expenses { get; set; } = new Expense();
        public List<Expense> ExpensesList { get; set; } = new List<Expense>();
        public List<ExpenseCategoryModel> expenseCategories { get; set; } = new List<ExpenseCategoryModel>();
        public List<ExpenseItemTransection> ItemTransection { get; set; } = new List<ExpenseItemTransection>();
        public List<ExpenseItemMaster> ExpenseDropDownItem { get; set; } = new List<ExpenseItemMaster>();
        public List<string> PaymentTypes { get; set; } = new List<string> { "Cash", "Credit", "Debit Card", "UPI", "Net Banking" };

        public List<Expense> CategoryTotals { get; set; } = new List<Expense>();
        public int SelectedCategoryId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalBalance { get; set; }
    }

  

    public class AddExpenseViewModel
    {

        [StringLength(100)]
        public string ItemName { get; set; }

        [StringLength(50)]
        public string ItemHsnSac { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }


        [StringLength(20)]
        public string TaxType { get; set; } = "Tax Excluded";


        [StringLength(20)]
        public string TaxRate { get; set; } = "IGST@0.25%";


        public string Category { get; set; } = "Petrol";
    }

    public class ExpenseCategoryModel
    {
        public int id { get; set; }
        public string Category { get; set; }
        public string ExpenseType { get; set; }
    }
    public class ExpenseItemTransection
    {
       
        public int Id { get; set; }

        public int ExpenseId { get; set; }

        public int ItemId { get; set; }
        public string itemname { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Amount { get; set; }
        public string expenseno { get; set; }
        public DateTime? expensedate { get; set; }
        public string Category { get; set; }
    }
    public class ExpenseItemMaster
    {
        public int Id { get; set; }       
        public string Name { get; set; }      
        public string HsnSacCode { get; set; }     
        public decimal Price { get; set; }      
        public string TaxType { get; set; }       
        public string TaxRate { get; set; }       
        public int StatusId { get; set; }
    }

}

