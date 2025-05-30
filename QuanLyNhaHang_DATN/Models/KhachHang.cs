﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QuanLyNhaHang_DATN.Models
{
    public class KhachHang
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        public string TenKhachHang { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải có đúng 10 chữ số")]
        public string SDT { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        public string? DiaChi { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public int? TaiKhoanId { get; set; }

        [ForeignKey("TaiKhoanId")]
        [ValidateNever]
        public TaiKhoan TaiKhoan { get; set; }
    }
}
