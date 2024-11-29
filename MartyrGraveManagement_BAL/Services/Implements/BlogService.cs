﻿using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.BlogDTOs;
using MartyrGraveManagement_BAL.ModelViews.TaskDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class BlogService: IBlogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BlogService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<List<DetailedBlogResponseDTO>> GetBlogByAccountId(int accountId)
        {
            // Kiểm tra xem AccountId có tồn tại trong hệ thống không
            var accountExists = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
            if (accountExists == null)
            {
                throw new KeyNotFoundException("Account không tồn tại.");
            }

            // Lấy các blog thuộc về account đã xác thực
            var blogs = await _unitOfWork.BlogRepository.GetAsync(
                filter: b => b.AccountId == accountId,
                includeProperties: "Account,HistoricalEvent,HistoricalImages,Comments,HistoricalRelatedMartyrs"
            );

            // Chuyển đổi các blog lấy được sang dạng DTO để trả về
            var blogResponses = blogs.Select(blog => new DetailedBlogResponseDTO
            {
                BlogId = blog.BlogId,
                AccountId = blog.AccountId,
                HistoryId = blog.HistoryId,
                BlogName = blog.BlogName,
                BlogDescription = blog.BlogDescription,
                BlogContent = blog.BlogContent,
                CreateDate = blog.CreateDate,
                UpdateDate = blog.UpdateDate,
                Status = blog.Status,
                FullName = blog.Account?.FullName, // Tên đầy đủ từ Account
                BlogCategoryName = blog.HistoricalEvent?.BlogCategoryName, // Tên sự kiện lịch sử từ HistoricalEvent
                HistoricalImages = blog.HistoricalImages?.Select(img => img.ImagePath) // Các đường dẫn hình ảnh từ HistoricalImages
            }).ToList();

            return blogResponses;
        }


        public async Task<string> CreateBlogAsync(CreateBlogDTORequest request, int accountId)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Kiểm tra xem AccountId có tồn tại không
                    var accountExists = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                    if (accountExists == null)
                    {
                        throw new KeyNotFoundException("Account không tồn tại.");
                    }

                    // Kiểm tra xem HistoryId có tồn tại và có trạng thái là true không
                    var blogCategory = await _unitOfWork.BlogCategoryRepository.GetByIDAsync(request.HistoryId);
                    if (blogCategory == null || !blogCategory.Status)
                    {
                        throw new KeyNotFoundException("BlogCategory không tồn tại hoặc không hợp lệ (trạng thái không phải là true).");
                    }

                    // Kiểm tra tính hợp lệ của các RelatedMartyrIds (nếu có)
                    if (request.RelatedMartyrIds != null && request.RelatedMartyrIds.Any())
                    {
                        foreach (var martyrId in request.RelatedMartyrIds)
                        {
                            var martyrExists = await _unitOfWork.MartyrGraveInformationRepository.GetByIDAsync(martyrId);
                            if (martyrExists == null)
                            {
                                throw new KeyNotFoundException($"Martyr với ID {martyrId} không tồn tại.");
                            }
                        }
                    }

                    // 1. Tạo Blog mới từ request
                    var newBlog = new Blog
                    {
                        AccountId = accountId,  // Sử dụng AccountId từ token
                        HistoryId = request.HistoryId,
                        BlogName = request.BlogName,
                        BlogDescription = request.BlogDescription,
                        BlogContent = request.BlogContent,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Status = false
                    };

                    // 2. Thêm Blog vào cơ sở dữ liệu để có được BlogId
                    await _unitOfWork.BlogRepository.AddAsync(newBlog);
                    await _unitOfWork.SaveAsync();

                    // 3. Thêm các HistoricalImages nếu có
                    if (request.HistoricalImageUrls != null && request.HistoricalImageUrls.Any())
                    {
                        var historicalImages = request.HistoricalImageUrls.Select(url => new HistoricalImage
                        {
                            BlogId = newBlog.BlogId,
                            ImagePath = url
                        }).ToList();

                        await _unitOfWork.HistoricalImageRepository.AddRangeAsync(historicalImages);
                    }

                    // 4. Thêm các HistoricalRelatedMartyrs nếu có
                    if (request.RelatedMartyrIds != null && request.RelatedMartyrIds.Any())
                    {
                        var historicalRelatedMartyrs = request.RelatedMartyrIds.Select(martyrId => new HistoricalRelatedMartyr
                        {
                            BlogId = newBlog.BlogId,
                            InformationId = martyrId,
                            Status = true
                        }).ToList();

                        await _unitOfWork.HistoricalRelatedMartyrRepository.AddRangeAsync(historicalRelatedMartyrs);
                    }

                    // 5. Commit transaction nếu không có lỗi
                    await transaction.CommitAsync();

                    return "Blog created successfully.";
                }
                catch (Exception ex)
                {
                    // Rollback transaction nếu có lỗi
                    await transaction.RollbackAsync();
                    throw new Exception($"Failed to create blog: {ex.Message}");
                }
            }
        }





        public async Task<(IEnumerable<BlogDTO> blogList, int totalPage)> GetAllBlogsAsync(int managerId, int pageIndex, int pageSize, DateTime Date)
        {
            //var blogs = await _unitOfWork.BlogRepository.GetAllAsync(includeProperties: "Account,HistoricalEvent");
            //var blogDtos = new List<BlogDTO>();

            //foreach (var blog in blogs)
            //{
            //    blogDtos.Add(new BlogDTO
            //    {
            //        BlogId = blog.BlogId,
            //        BlogName = blog.BlogName,
            //        BlogDescription = blog.BlogDescription,
            //        BlogContent = blog.BlogContent,
            //        AccountId = blog.AccountId,
            //        FullName = blog.Account?.FullName,
            //        HistoryId = blog.HistoryId,
            //        BlogCategoryName = blog.HistoricalEvent?.BlogCategoryName,
            //        CreateDate = blog.CreateDate,
            //        UpdateDate = blog.UpdateDate,
            //        Status = blog.Status
            //    });
            //}

            //return blogDtos;
            try
            {
                // Kiểm tra xem AccountId có tồn tại không
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(managerId);
                if (account == null)
                {
                    throw new KeyNotFoundException("Account not found.");
                }

                int totalPage = 0;
                int totalBlog = 0;
                IEnumerable<Blog> blogs = new List<Blog>();
                if (Date == DateTime.MinValue)
                {
                    totalBlog = (await _unitOfWork.BlogRepository.GetAsync(s => s.Account.AreaId == account.AreaId, includeProperties: "Account,HistoricalEvent")).Count();
                    totalPage = (int)Math.Ceiling(totalBlog / (double)pageSize);
                    // Lấy tất cả các đơn hàng dựa trên AccountId và bao gồm các chi tiết đơn hàng
                    blogs = await _unitOfWork.BlogRepository.GetAsync(s => s.Account.AreaId == account.AreaId, includeProperties: "Account,HistoricalEvent", orderBy: q => q.OrderByDescending(s => s.UpdateDate),
                    pageIndex: pageIndex, pageSize: pageSize);
                }
                else
                {
                    totalBlog = (await _unitOfWork.BlogRepository.GetAsync(s => s.Account.AreaId == account.AreaId && s.UpdateDate.Date == Date.Date, includeProperties: "Account,HistoricalEvent")).Count();
                    totalPage = (int)Math.Ceiling(totalBlog / (double)pageSize);
                    // Lấy tất cả các đơn hàng dựa trên AccountId và bao gồm các chi tiết đơn hàng
                    blogs = await _unitOfWork.BlogRepository.GetAsync(t => t.Account.AreaId == account.AreaId && t.UpdateDate.Date == Date.Date, includeProperties: "Account,HistoricalEvent", orderBy: q => q.OrderByDescending(s => s.UpdateDate),
                    pageIndex: pageIndex, pageSize: pageSize);
                }


                // Lấy danh sách các Task thuộc về account, bao gồm các bảng liên quan


                if (!blogs.Any())
                {
                    throw new InvalidOperationException("This account does not have any tasks.");
                }

                // Ánh xạ FullName từ Account và các thông tin khác
                var blogResponses = new List<BlogDTO>();
                foreach (var blog in blogs)
                {
                    blogResponses.Add(new BlogDTO
                    {
                        BlogId = blog.BlogId,
                        BlogName = blog.BlogName,
                        BlogDescription = blog.BlogDescription,
                        BlogContent = blog.BlogContent,
                        AccountId = blog.AccountId,
                        FullName = blog.Account?.FullName,
                        HistoryId = blog.HistoryId,
                        BlogCategoryName = blog.HistoricalEvent?.BlogCategoryName,
                        CreateDate = blog.CreateDate,
                        UpdateDate = blog.UpdateDate,
                        Status = blog.Status
                    });
                }

                return (blogResponses, totalPage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(List<BlogDTO> blogList, int totalPage)> GetAllBlogsWithStatusTrueAsync(int pageIndex = 1, int pageSize = 5)
        {
            // Lấy tất cả các blog có status = true với phân trang
            var blogs = await _unitOfWork.BlogRepository.GetAsync(
                filter: b => b.Status == true,
                includeProperties: "Account,HistoricalEvent,HistoricalImages",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            // Đếm tổng số lượng blog có status = true
            var totalBlogs = (await _unitOfWork.BlogRepository.GetAsync(
                filter: b => b.Status == true)).Count();

            // Tính tổng số trang
            var totalPage = (int)Math.Ceiling(totalBlogs / (double)pageSize);

            // Chuyển đổi dữ liệu sang danh sách BlogDTO
            var blogDTOs = blogs.Select(blog => new BlogDTO
            {
                BlogId = blog.BlogId,
                BlogName = blog.BlogName,
                BlogDescription = blog.BlogDescription,
                BlogContent = blog.BlogContent,
                AccountId = blog.AccountId,
                FullName = blog.Account?.FullName,
                HistoryId = blog.HistoryId,
                BlogCategoryName = blog.HistoricalEvent?.BlogCategoryName,
                CreateDate = blog.CreateDate,
                UpdateDate = blog.UpdateDate,
                Status = blog.Status,

                // Danh sách hình ảnh lịch sử liên kết với blog
                HistoricalImages = blog.HistoricalImages?.Select(img => img.ImagePath).ToList()
            }).ToList();

            return (blogDTOs, totalPage);
        }





        public async Task<BlogDTO> GetBlogByIdAsync(int blogId)
        {
            var blog = await _unitOfWork.BlogRepository.GetAsync(
                filter: b => b.BlogId == blogId,
                includeProperties: "Account,HistoricalEvent,HistoricalImages,HistoricalRelatedMartyrs.MartyrGraveInformation.MartyrGrave.GraveImages,Comments.Account,Comments.Comment_Icons.Icon,Comments.Comment_Icons.Account"
            );

            var blogEntity = blog.FirstOrDefault();
            if (blogEntity == null) return null;

            return new BlogDTO
            {
                BlogId = blogEntity.BlogId,
                BlogName = blogEntity.BlogName,
                BlogDescription = blogEntity.BlogDescription,
                BlogContent = blogEntity.BlogContent,
                AccountId = blogEntity.AccountId,
                FullName = blogEntity.Account?.FullName,
                HistoryId = blogEntity.HistoryId,
                BlogCategoryName = blogEntity.HistoricalEvent?.BlogCategoryName,
                CreateDate = blogEntity.CreateDate,
                UpdateDate = blogEntity.UpdateDate,
                Status = blogEntity.Status,

                HistoricalImages = blogEntity.HistoricalImages?.Select(img => img.ImagePath).ToList(),

                RelatedMartyrDetails = blogEntity.HistoricalRelatedMartyrs?
                    .Where(hrm => hrm.MartyrGraveInformation != null)
                    .Select(hrm => new MartyrDetailDTO
                    {
                        MartyrGraveId = hrm.InformationId,
                        Name = hrm.MartyrGraveInformation.Name,
                        Images = hrm.MartyrGraveInformation.MartyrGrave?.GraveImages?.Select(img => img.UrlPath).ToList()
                    })
                    .ToList(),

                Comments = blogEntity.Comments?
                    .Where(comment => comment.Status == true) 
                    .Select(comment => new CommentDetailDTO
                    {
                        CommentId = comment.CommentId,
                        Content = comment.Content,
                        CreatedDate = comment.CreatedDate,
                        UpdatedDate = comment.UpdatedDate,
                        AccountId = comment.Account.AccountId,
                        AccountName = comment.Account?.FullName,
                        AccountAvatar = comment.Account?.AvatarPath,

                        CommentIcons = comment.Comment_Icons
                            .GroupBy(icon => icon.Icon?.IconImage)
                            .Select(group => new CommentIconDetailDTO
                            {
                                Id = group.First().Id,           
                                IconId = group.First().IconId,   
                                IconImage = group.Key,
                                Count = group.Count(),           
                                AccountNames = group.Select(icon => icon.Account?.FullName)
                                    .Where(name => !string.IsNullOrEmpty(name)).ToList() 
                            })
                            .ToList()
                    }).ToList()
            };
        }








        public async Task<string> UpdateBlogAsync(int blogId, CreateBlogDTORequest request)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Lấy blog từ database
                    var blog = await _unitOfWork.BlogRepository.GetByIDAsync(blogId);
                    if (blog == null)
                    {
                        return "Blog không tồn tại.";
                    }

                    // Cập nhật thông tin blog
                    blog.BlogName = request.BlogName;
                    blog.BlogDescription = request.BlogDescription;
                    blog.BlogContent = request.BlogContent;
                    blog.UpdateDate = DateTime.Now;
                    blog.Status = false;
                    // Cập nhật HistoricalImages
                    if (request.HistoricalImageUrls != null)
                    {
                        // Xóa các HistoricalImages cũ
                        var existingImages = await _unitOfWork.HistoricalImageRepository.FindAsync(h => h.BlogId == blogId);
                        await _unitOfWork.HistoricalImageRepository.DeleteRangeAsync(existingImages);

                        // Thêm các HistoricalImages mới
                        foreach (var url in request.HistoricalImageUrls)
                        {
                            var historicalImage = new HistoricalImage
                            {
                                BlogId = blogId,
                                ImagePath = url
                            };
                            await _unitOfWork.HistoricalImageRepository.AddAsync(historicalImage);
                        }
                    }

                    // Cập nhật HistoricalRelatedMartyrs
                    if (request.RelatedMartyrIds != null)
                    {
                        // Xóa các HistoricalRelatedMartyrs cũ
                        var existingRelatedMartyrs = await _unitOfWork.HistoricalRelatedMartyrRepository.FindAsync(h => h.BlogId == blogId);
                        await _unitOfWork.HistoricalRelatedMartyrRepository.DeleteRangeAsync(existingRelatedMartyrs);

                        // Thêm các HistoricalRelatedMartyrs mới
                        foreach (var martyrId in request.RelatedMartyrIds)
                        {
                            var historicalRelatedMartyr = new HistoricalRelatedMartyr
                            {
                                BlogId = blogId,
                                InformationId = martyrId,
                                Status = true
                            };
                            await _unitOfWork.HistoricalRelatedMartyrRepository.AddAsync(historicalRelatedMartyr);
                        }
                    }

                    // Lưu các thay đổi
                    await _unitOfWork.BlogRepository.UpdateAsync(blog);
                    await _unitOfWork.SaveAsync();

                    // Commit transaction nếu không có lỗi
                    await transaction.CommitAsync();

                    return "Cập nhật Blog thành công.";
                }
                catch (Exception ex)
                {
                    // Rollback transaction nếu có lỗi
                    await transaction.RollbackAsync();
                    throw new Exception($"Failed to update blog: {ex.Message}");
                }
            }
        }

        public async Task<string> UpdateBlogStatusAsync(int blogId, bool status)
        {
            var blog = await _unitOfWork.BlogRepository.GetByIDAsync(blogId);
            if (blog == null)
            {
                return "Blog không tồn tại.";
            }

            // Cập nhật trạng thái blog
            blog.Status = status;
            blog.UpdateDate = DateTime.Now;

            await _unitOfWork.BlogRepository.UpdateAsync(blog);
            await _unitOfWork.SaveAsync();

            return "Cập nhật trạng thái Blog thành công.";
        }
    }
}
