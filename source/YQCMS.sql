-- �½����ݿ�
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
--���±�
create table dbo.cms_article
(
id int identity(1,1) primary key,
typeid int not null constraint df_article_typeid default(0),--����id
cateid int not null constraint df_article_cateid default(0),--���id
catepath nvarchar(50) not null constraint df_article_catepath default('0'),--������������·�� ��eg.0,1,124
articleid int not null constraint df_article_articleid default(0),--����id ֵΪĳid
parentid int not null constraint df_article_parentid default(0),--��id  ֵΪĳid
layer int not null constraint df_article_layer default(0),--�㣬eg.����ֵΪ0�����»ظ�1���ظ��ٻظ�2
subcount int not null constraint df_article_subcount default(0),--������ͳ����
catename nvarchar(50) not null constraint df_article_catename default('0'),-- ���﷽���ȡ���ݼ����������
userid int not null constraint df_article_userid default(0),--�û�ID
username nchar(20) not null constraint df_article_username default(''),--�û���
title nvarchar(200) not null constraint df_article_title default(''),--����
summary nvarchar(500) not null constraint df_article_summary default(''),--ժҪ
content ntext not null constraint df_article_content default(''),--����
viewcount int not null constraint df_article_viewcount default(0),--���ͳ��
orderid int not null constraint df_article_orderid default(1),--����
replypermit tinyint not null constraint df_article_replypermit default(1),--�Ƿ�ɻظ�,1-��,0-����
status tinyint not null constraint df_article_status default(0),--״̬,Ӧ�����ܵ�ɾ��,���εȲ���
ip nvarchar(20) not null constraint df_article_ip default(''),--ip
favor int not null constraint df_article_favor default(0),--֧��
against int not null constraint df_article_against default(0),--����
iscommend tinyint not null constraint df_article_iscommend default(0),--�Ƿ��Ƽ�
istop tinyint not null constraint df_article_istop default(0),--�Ƿ��ö�
createdate datetime not null constraint df_article_createdate default(getdate()),--����ʱ��
lastreplydate datetime not null constraint df_article_lastreplydate default(getdate()),--���ظ�ʱ��
lastreplyuser nchar(20) not null constraint df_article_lastreplyuser default('')--���ظ��û���
)
GO
--����������Ϣ��
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
--����������ͼ
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
--����¼��洢����
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
--�����޸Ĵ洢����
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
    --�޸����»�����Ϣ
    update cms_article set typeid=@typeid,cateid=@cateid,catename=@catename,catepath=@catepath,title=@title,summary=@summary,[content]=@content,replypermit=@replypermit,[status]=@status,iscommend=@iscommend,[istop]=@istop where id=@aid
    --�޸�seo����
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
--����ɾ���洢����
create procedure [dbo].[cms_deletearticle]
@aid int,
@parentid int
as
begin    
	if @parentid=0        
		begin            
			--ɾ�����»�����Ϣ�Լ����Ӽ�¼
			delete from cms_article where articleid=@aid            
			--ɾ��seo����           
			delete from cms_articledetail where articleid=@aid        
		end    
	else        
		begin            
			--ɾ�����»�����Ϣ           
			delete from cms_article where id=@aid  
			--���¸���¼��Ϣ   
			update cms_article set subcount=(select count(1) from cms_article where parentid=@parentid) where id=@parentid
			--ɾ�������е��Ӽ�¼           
			while((select count(1) from cms_article where parentid<>0 and parentid not in (select id from article))>0)            
				begin                
				delete from article where parentid<>0 and not exists(select 1 from article b where b.id=article.parentid)            
				end         
		end   
	select @@ERROR
end