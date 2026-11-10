# CẨM NANG TRUY VẤN ODATA V8 (ODATA CHEAT SHEET)
> **Dự án**: FU News Management System (PRN232)  
> **Giao thức**: ASP.NET Core OData v8 + RESTful API  
> **Cổng mặc định**: `https://localhost:7000` (HTTPS) hoặc `http://localhost:5000` (HTTP)

---

## 📌 MỤC LỤC
1. [Địa chỉ gốc & Metadata](#1-địa-chỉ-gốc--metadata)
2. [Các EntitySet được hỗ trợ](#2-các-entityset-được-hỗ-trợ)
3. [Truy vấn cơ bản](#3-truy-vấn-cơ-bản)
4. [Phân trang ($top, $skip)](#4-phân-trang-top-skip)
5. [Sắp xếp ($orderby)](#5-sắp-xếp-orderby)
6. [Lọc dữ liệu ($filter)](#6-lọc-dữ-liệu-filter)
7. [Chọn cột cần lấy ($select)](#7-chọn-cột-cần-lấy-select)
8. [Đếm số lượng ($count)](#8-đếm-số-lượng-count)
9. [Các truy vấn kết hợp thực tế (Real-world Examples)](#9-các-truy-vấn-kết-hợp-thực-tế)
10. [Bảng tra cứu toán tử OData](#10-bảng-tra-cứu-toán-tử-odata)

---

## 1. Địa chỉ gốc & Metadata

- **Xem toàn bộ cấu trúc Schema XML (EDM Model)**:
  ```http
  GET https://localhost:7000/odata/$metadata
  ```
- **Service Root Endpoint**:
  ```http
  GET https://localhost:7000/odata
  ```

---

## 2. Các EntitySet được hỗ trợ

| EntitySet | Mô tả | Quyền truy cập |
| :--- | :--- | :--- |
| `NewsArticles` | Quản lý bài báo tin tức | Công khai (Public) đối với tin Active |
| `Categories` | Quản lý danh mục | Xem công khai / Staff quản lý |
| `Accounts` | Quản lý tài khoản | Bắt buộc Token quyền **Admin** |
| `Tags` | Quản lý thẻ bài viết | Xem công khai / Staff quản lý |

> **Lưu ý về URL**: Hệ thống hỗ trợ song song cả 2 định dạng route:
> - Theo chuẩn OData: `https://localhost:7000/odata/[EntitySet]`
> - Theo chuẩn RESTful API: `https://localhost:7000/api/[EntitySet]`

---

## 3. Truy vấn cơ bản

### 3.1. Lấy toàn bộ danh sách bài báo
```http
GET https://localhost:7000/odata/NewsArticles
```
hoặc: 
```http
GET https://localhost:7000/api/NewsArticles
```

### 3.2. Lấy chi tiết 1 bài báo theo ID
```http
GET https://localhost:7000/api/NewsArticles/2
```

### 3.3. Lấy toàn bộ danh mục (Categories)
```http
GET https://localhost:7000/odata/Categories
```

---

## 4. Phân trang ($top, $skip)

### 4.1. Lấy Top 3 bài báo đầu tiên
```http
GET https://localhost:7000/odata/NewsArticles?$top=3
```

### 4.2. Bỏ qua 3 bài đầu, lấy 3 bài tiếp theo (Trang 2)
```http
GET https://localhost:7000/odata/NewsArticles?$skip=3&$top=3
```

### 4.3. Lấy trang 3 với kích thước 5 bài/trang (PageSize = 5, PageIndex = 3)
*Công thức: `$skip = (PageIndex - 1) * PageSize`*
```http
GET https://localhost:7000/odata/NewsArticles?$skip=10&$top=5
```

---

## 5. Sắp xếp ($orderby)

### 5.1. Hiển thị bài báo mới nhất (Ngày tạo giảm dần - Descending)
```http
GET https://localhost:7000/odata/NewsArticles?$orderby=createdDate desc
```

### 5.2. Hiển thị Top 3 bài báo mới nhất
```http
GET https://localhost:7000/odata/NewsArticles?$orderby=createdDate desc&$top=3
```

### 5.3. Sắp xếp bài báo theo tiêu đề từ A -> Z (Ascending)
```http
GET https://localhost:7000/odata/NewsArticles?$orderby=newsTitle asc
```

### 5.4. Sắp xếp nhiều tiêu chí (Theo Category tăng dần, trong cùng Category thì ngày tạo giảm dần)
```http
GET https://localhost:7000/odata/NewsArticles?$orderby=categoryId asc,createdDate desc
```

---

## 6. Lọc dữ liệu ($filter)

### 6.1. Lọc theo trạng thái bài viết đang hoạt động (`newsStatus = true`)
```http
GET https://localhost:7000/odata/NewsArticles?$filter=newsStatus eq true
```

### 6.2. Lọc bài báo thuộc một danh mục cụ thể (Ví dụ CategoryId = 1)
```http
GET https://localhost:7000/odata/NewsArticles?$filter=categoryId eq 1
```

### 6.3. Tìm kiếm bài báo có tiêu đề chứa từ khóa (Hàm `contains`)
*Tìm bài báo có tiêu đề chứa chữ "Alumni":*
```http
GET https://localhost:7000/odata/NewsArticles?$filter=contains(newsTitle, 'Alumni')
```

### 6.4. Tìm bài báo có tiêu đề bắt đầu bằng từ khóa (Hàm `startswith`)
```http
GET https://localhost:7000/odata/NewsArticles?$filter=startswith(newsTitle, 'Academic')
```

### 6.5. Lọc kết hợp nhiều điều kiện với `and`
*Lấy bài báo đang Active VÀ thuộc danh mục 1:*
```http
GET https://localhost:7000/odata/NewsArticles?$filter=newsStatus eq true and categoryId eq 1
```

### 6.6. Lọc điều kiện hoặc với `or`
*Lấy bài báo thuộc danh mục 1 HOẶC danh mục 2:*
```http
GET https://localhost:7000/odata/NewsArticles?$filter=categoryId eq 1 or categoryId eq 2
```

### 6.7. Lọc bài báo được tạo từ một ngày cụ thể trở đi
```http
GET https://localhost:7000/odata/NewsArticles?$filter=createdDate ge 2024-05-01T00:00:00Z
```

---

## 7. Chọn cột cần lấy ($select)

*Giúp giảm dung lượng gói tin truyền tải, chỉ lấy những thông tin cần thiết.*

### 7.1. Chỉ lấy ID, Tiêu đề và Ngày tạo của bài báo
```http
GET https://localhost:7000/odata/NewsArticles?$select=newsArticleId,newsTitle,createdDate
```

### 7.2. Lấy danh sách danh mục chỉ gồm Mã và Tên danh mục
```http
GET https://localhost:7000/odata/Categories?$select=categoryId,categoryName
```

---

## 8. Đếm số lượng ($count)

### 8.1. Đếm tổng số lượng bản ghi (trả về con số thuần túy)
```http
GET https://localhost:7000/odata/Categories/$count
```
```http
GET https://localhost:7000/odata/Accounts/$count
```

### 8.2. Lấy dữ liệu kèm theo tổng số lượng bản ghi (`$count=true`)
*Hữu ích cho việc làm thanh phân trang ở Client:*
```http
GET https://localhost:7000/odata/NewsArticles?$count=true&$top=3
```
*Kết quả trả về sẽ có thêm trường `@odata.count`:*
```json
{
  "@odata.context": "https://localhost:7000/odata/$metadata#NewsArticles",
  "@odata.count": 15,
  "value": [ ... 3 bài báo ... ]
}
```

---

## 9. Các truy vấn kết hợp thực tế

### 🌟 Kịch bản 1: Lấy Top 3 bài báo mới nhất đang Active của danh mục 1
```http
GET https://localhost:7000/odata/NewsArticles?$filter=newsStatus eq true and categoryId eq 1&$orderby=createdDate desc&$top=3
```

### 🌟 Kịch bản 2: Tìm kiếm bài báo chứa chữ "STEM", chỉ lấy tiêu đề và ngày tạo, sắp xếp mới nhất
```http
GET https://localhost:7000/odata/NewsArticles?$filter=contains(newsTitle, 'STEM')&$select=newsArticleId,newsTitle,createdDate&$orderby=createdDate desc
```

### 🌟 Kịch bản 3: Lấy danh sách Category đang hoạt động (isActive = true), sắp xếp theo tên A-Z
```http
GET https://localhost:7000/odata/Categories?$filter=isActive eq true&$orderby=categoryName asc
```

### 🌟 Kịch bản 4: (Quyền Admin) Lấy danh sách tài khoản Staff (AccountRole = 1), sắp xếp theo tên
*Yêu cầu Header: `Authorization: Bearer <JWT_TOKEN>`*
```http
GET https://localhost:7000/odata/Accounts?$filter=accountRole eq 1&$orderby=accountName asc&$select=accountId,accountName,accountEmail
```

---

## 10. Bảng tra cứu toán tử OData

### Toán tử so sánh (Comparison Operators)

| Toán tử | Ý nghĩa | Ví dụ OData |
| :---: | :--- | :--- |
| `eq` | Bằng (Equal) | `categoryId eq 1` |
| `ne` | Khác (Not Equal) | `newsStatus ne false` |
| `gt` | Lớn hơn (Greater Than) | `accountId gt 5` |
| `ge` | Lớn hơn hoặc bằng (Greater or Equal) | `createdDate ge 2024-01-01T00:00:00Z` |
| `lt` | Nhỏ hơn (Less Than) | `categoryId lt 10` |
| `le` | Nhỏ hơn hoặc bằng (Less or Equal) | `categoryId le 5` |

### Toán tử logic (Logical Operators)

| Toán tử | Ý nghĩa | Ví dụ OData |
| :---: | :--- | :--- |
| `and` | Và (Cả 2 vế đều đúng) | `newsStatus eq true and categoryId eq 1` |
| `or` | Hoặc (Một trong 2 vế đúng) | `categoryId eq 1 or categoryId eq 2` |
| `not` | Phủ định | `not (categoryId eq 1)` |

### Hàm xử lý chuỗi (String Functions)

| Hàm | Ý nghĩa | Ví dụ OData |
| :--- | :--- | :--- |
| `contains(cột, 'chuỗi')` | Kiểm tra chuỗi con | `contains(headline, 'Research')` |
| `startswith(cột, 'chuỗi')` | Bắt đầu bằng | `startswith(newsTitle, 'New')` |
| `endswith(cột, 'chuỗi')` | Kết thúc bằng | `endswith(newsTitle, 'Program')` |
| `tolower(cột)` | Chuyển thành chữ thường | `contains(tolower(newsTitle), 'fu')` |
| `toupper(cột)` | Chuyển thành chữ in hoa | `contains(toupper(newsTitle), 'FU')` |

