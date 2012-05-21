-- 新建数据库
--create database YQCMS

use	YQCMS

/*drop table dbo.cms_article
drop table dbo.cms_articledetail
drop view dbo.cms_varticle
drop procedure dbo.cms_createarticle
drop procedure dbo.cms_deletearticle
drop procedure dbo.cms_updatearticle
*/

GO
--文章表
create table dbo.cms_article
(
id int identity(1,1) primary key,
typeid int not null constraint df_article_typeid default(0),--类型id
cateid int not null constraint df_article_cateid default(0),--类别id
catepath nvarchar(50) not null constraint df_article_catepath default('0'),--基于类别的纵深路径 ，eg.0,1,124
articleid int not null constraint df_article_articleid default(0),--文章id 值为某id
parentid int not null constraint df_article_parentid default(0),--父id  值为某id
layer int not null constraint df_article_layer default(0),--层，eg.文章值为0，文章回复1，回复再回复2
subcount int not null constraint df_article_subcount default(0),--子数据统计数
catename nvarchar(50) not null constraint df_article_catename default('0'),-- 这里方便读取数据记入类别名称
userid int not null constraint df_article_userid default(0),--用户ID
username nchar(20) not null constraint df_article_username default(''),--用户名
title nvarchar(200) not null constraint df_article_title default(''),--标题
summary nvarchar(500) not null constraint df_article_summary default(''),--摘要
content ntext not null constraint df_article_content default(''),--内容
viewcount int not null constraint df_article_viewcount default(0),--浏览统计
orderid int not null constraint df_article_orderid default(1),--排序
replypermit tinyint not null constraint df_article_replypermit default(1),--是否可回复,1-可,0-不可
status tinyint not null constraint df_article_status default(0),--状态,应付可能的删除,屏蔽等操作
ip nvarchar(20) not null constraint df_article_ip default(''),--ip
favor int not null constraint df_article_favor default(0),--支持
against int not null constraint df_article_against default(0),--反对
iscommend tinyint not null constraint df_article_iscommend default(0),--是否推荐
istop tinyint not null constraint df_article_istop default(0),--是否置顶
createdate datetime not null constraint df_article_createdate default(getdate()),--创建时间
lastreplydate datetime not null constraint df_article_lastreplydate default(getdate()),--最后回复时间
lastreplyuser nchar(20) not null constraint df_article_lastreplyuser default('')--最后回复用户名
)
GO
--文章扩充信息表
create table dbo.cms_articledetail
(
articleid int not null constraint df_detail_articleid default(0),
seotitle nvarchar(500) not null constraint df_detail_title default(''),
seodescription nvarchar(1000) not null constraint df_detail_description default(''),
seokeywords nvarchar(500) not null constraint df_detail_keywords default(''),
seometas nvarchar(1000) not null constraint df_detail_metas default(''),
rename nvarchar(60) not null constraint df_detail_rename default(''),
tags nvarchar(60) not null constraint df_detail_tags default('')
)

GO
--创建文章视图
create view dbo.cms_varticle
as
select a.*,
isnull(seotitle,'') as seotitle,
isnull(seodescription,'') as seodescription,
isnull(seokeywords,'') as seokeywords,
isnull(seometas,'') as seometas,
isnull(rename,'') as rename,
isnull(tags,'') as tags,
(case isnull(b.rename,'') when '' then 'archive/' + cast(a.id as varchar) else 'article/' + b.rename end) as url
from cms_article a
left join cms_articledetail b
on a.id=b.articleid
where a.layer=0


GO
--文章录入存储过程
create procedure dbo.cms_createarticle
@typeid int,
@cateid int,
@catepath nvarchar(100),
@articleid int,
@parentid int,
@layer int,
@catename nvarchar(100),
@userid int,
@username nchar(20),
@title nvarchar(200),
@summary nvarchar(500),
@content ntext,
@replypermit tinyint,
@status tinyint,
@ip nvarchar(20),
@seotitle nvarchar(500),
@seodescription nvarchar(1000),
@seokeywords nvarchar(500),
@seometas nvarchar(1000),
@rename nvarchar(60),
@tags nvarchar(60),
@iscommend tinyint,
@istop tinyint

as

declare @aid int

insert into 
cms_article(typeid,cateid ,catepath ,articleid ,parentid ,layer ,catename ,userid ,username ,title ,summary ,[content], replypermit, [status], ip ,lastreplyuser,iscommend,istop)
values(@typeid,@cateid ,@catepath ,@articleid ,@parentid ,@layer ,@catename ,@userid ,@username ,@title ,@summary ,@content, @replypermit, @status ,@ip ,@username,@iscommend,@istop )

set @aid=SCOPE_IDENTITY()

if @@ERROR=0
begin
    if @layer=0
        begin
            update cms_article set articleid=@aid WHERE id=@aid
            if(LTRIM(@seotitle+@seodescription+@seokeywords+@seometas+@rename+@tags)<>'')
				begin
					insert into cms_articledetail(articleid,seotitle,seodescription,seokeywords,seometas,rename,tags) VALUES(@aid,@seotitle,@seodescription,@seokeywords,@seometas,@rename,@tags)
				end
        end
    else
        begin
			update cms_article 
			set subcount=(select count(1) from cms_article where parentid=@parentid),
			lastreplydate=getdate(),lastreplyuser=@username
			where id=@parentid
        end
end

SELECT @aid as articleid

GO
--文章修改存储过程
create procedure [dbo].[cms_updatearticle]
@aid int,
@typeid int,
@cateid int,
@catename nvarchar(100),
@catepath nvarchar(200),
@parentid int,
@title nvarchar(200),
@summary nvarchar(500),
@content ntext,
@replypermit tinyint,
@status tinyint,
@seotitle nvarchar(500),
@seodescription nvarchar(1000),
@seokeywords nvarchar(500),
@seometas nvarchar(1000),
@rename nvarchar(60),
@tags nvarchar(60),
@iscommend tinyint,
@istop tinyint

as
begin
    --修改文章基础信息
    update cms_article set typeid=@typeid,cateid=@cateid,catename=@catename,catepath=@catepath,title=@title,summary=@summary,[content]=@content,replypermit=@replypermit,[status]=@status,iscommend=@iscommend,[istop]=@istop where id=@aid
    --修改seo部分
    if @parentid=0
        begin
            delete from cms_articledetail where articleid=@aid
            if(LTRIM(@seotitle+@seodescription+@seokeywords+@seometas+@rename+@tags)<>'')
				begin
					insert into cms_articledetail(articleid,seotitle,seodescription,seokeywords,seometas,rename,tags) VALUES(@aid,@seotitle,@seodescription,@seokeywords,@seometas,@rename,@tags)
				end
        end
   select @@ERROR
end

GO
--文章删除存储过程
create procedure [dbo].[cms_deletearticle]
@aid int,
@parentid int
as
begin    
	if @parentid=0        
		begin            
			--删除文章基础信息以及其子记录
			delete from cms_article where articleid=@aid            
			--删除seo部分           
			delete from cms_articledetail where articleid=@aid        
		end    
	else        
		begin            
			--删除文章基础信息           
			delete from cms_article where id=@aid  
			--更新父记录信息   
			update cms_article set subcount=(select count(1) from cms_article where parentid=@parentid) where id=@parentid
			--删除可能有的子记录           
			while((select count(1) from cms_article where parentid<>0 and parentid not in (select id from article))>0)            
				begin                
				delete from article where parentid<>0 and not exists(select 1 from article b where b.id=article.parentid)            
				end         
		end   
	select @@ERROR
end