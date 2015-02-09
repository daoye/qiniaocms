--角色
INSERT INTO "role" ("id", "siteid", "name") VALUES ('1', '0', '超级管理员');
INSERT INTO "role" ("id", "siteid", "name") VALUES ('2', '0', '管理员');
INSERT INTO "role" ("id", "siteid", "name") VALUES ('3', '0', '编辑者');
INSERT INTO "role" ("id", "siteid", "name") VALUES ('4', '0', '普通用户');

--菜单
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('1', '文章', '0', 'list', 'posts', 'admin', '0', '1', '<i class="menu-icon fa fa-edit"></i>', 'Operate,Delete,Update', 'postlist');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('2', '撰写文章', '0', 'add', 'posts', 'admin', '1', '1', NULL, 'Operate,Delete,Update', 'postadd');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('3', '所有文章', '0', 'list', 'posts', 'admin', '1', '2', NULL, 'Operate,Delete,Update', 'postlist');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('4', '相册', '0', 'list', 'albums', 'admin', '0', '2', '<i class="menu-icon fa fa-picture-o"></i>', 'Operate,Delete,Update', 'albumlist');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('5', '添加相册', '0', 'add', 'albums', 'admin', '4', '1', NULL, 'Operate,Delete,Update,UpdatePost,PostList,PostListItem,OperatePost', 'albumadd');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('6', '所有相册', '0', 'list', 'albums', 'admin', '4', '2', NULL, 'Operate,Delete,Update,UpdatePost,PostList,PostListItem,OperatePost', 'albumlist');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('7', '网站', '0', 'list', 'sites', 'admin', '0', '3', '<i class="menu-icon fa fa-star"></i>', 'Operate,Delete,Update', 'sitelist');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('8', '新建网站', '0', 'add', 'sites', 'admin', '7', '1', NULL, 'Operate,Delete,Update', 'siteadd');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('9', '所有网站', '0', 'list', 'sites', 'admin', '7', '2', NULL, 'Operate,Delete,Update', 'sitelist');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('10', '页面', '0', 'list', 'pages', 'admin', '0', '4', '<i class="menu-icon fa fa-pencil"></i>', 'Operate,Delete,Update', 'pagelist');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('11', '新建页面', '0', 'add', 'pages', 'admin', '10', '1', NULL, 'Operate,Delete,Update', 'pageadd');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('12', '所有页面', '0', 'list', 'pages', 'admin', '10', '2', NULL, 'Operate,Delete,Update', 'pagelist');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('13', '文件', '0', 'list', 'medias', 'admin', '0', '5', '<i class="menu-icon fa fa-folder"></i>', 'Operate,Delete,Update', 'medialist');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('14', '新文件', '0', 'add', 'medias', 'admin', '13', '1', NULL, 'Operate,Delete,Update', 'mediaadd');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('15', '所有文件', '0', 'list', 'medias', 'admin', '13', '2', NULL, 'Operate,Delete,Update', 'medialist');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('16', '外观', '0', 'list', 'themes', 'admin', '0', '6', '<i class="menu-icon fa fa-eye"></i>', 'Detail,SetDefault,Add,Delete,List,LocalInstall', 'themelist');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('17', '主题', '0', 'list', 'themes', 'admin', '16', '1', NULL, 'Detail,SetDefault,Add,Delete,List,LocalInstall', 'themelist');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('18', '菜单', '0', 'list', 'navs', 'admin', '16', '2', NULL, 'Add,List,Update,Delete,SetDefault,Operate', 'navlist');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('19', '编辑', '0', 'edit', 'themes', 'admin', '16', '3', NULL, '', 'edittheme');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('20', '用户', '0', 'list', 'users', 'admin', '0', '7', '<i class="menu-icon fa fa-users"></i>', 'Operate,Delete,Update', 'userlist');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('21', '新用户', '0', 'add', 'users', 'admin', '20', '1', NULL, 'Operate,Delete,Update', 'useradd');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('22', '所有用户', '0', 'list', 'users', 'admin', '20', '2', NULL, 'Operate,Delete,Update', 'userlist');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('23', '新角色', '0', 'add', 'roles', 'admin', '20', '3', NULL, 'Operate,Delete,Update', 'roleadd');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('24', '所有角色', '0', 'list', 'roles', 'admin', '20', '4', NULL, 'Operate,Delete,Update', 'rolelist');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('25', '文章分类', '0', 'posts', 'terms', 'admin', '1', '1', NULL, 'AddPost,UpdatePost,Operate,Delete', 'termlist');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('26', '评论', '0', 'list', 'comments', 'admin', '0', '5', '<i class="menu-icon fa fa-comments"></i>', 'Update,Delete,Operate', 'commentlist');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('27', '个人信息', '0', 'update', 'mine', 'admin', '0', '9', '<i class="menu-icon fa fa-user"></i>', '', 'updatemine');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('28', '个人资料', '0', 'update', 'mine', 'admin', '27', '1', NULL, NULL, 'updatemine');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('29', '修改密码', '0', 'modifypass', 'mine', 'admin', '27', '2', NULL, 'PassIsRight', 'modifypass');
INSERT INTO "carte" ("id", "name", "siteid", "action", "controller", "area", "parent", "order", "icon", "allowactions", "activeflag") VALUES ('30', '网站设置', '0', 'current', 'sites', 'admin', '0', '10', '<i class="menu-icon fa fa-cog"></i>', 'DomainExists', 'sitescurrent');

--权限
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('1', '1', '1');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('2', '1', '2');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('3', '1', '3');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('4', '1', '4');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('5', '1', '5');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('6', '1', '6');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('7', '1', '7');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('8', '1', '8');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('9', '1', '9');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('10', '1', '10');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('11', '1', '11');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('12', '1', '12');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('13', '1', '13');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('14', '1', '14');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('15', '1', '15');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('16', '1', '16');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('17', '1', '17');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('18', '1', '18');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('19', '1', '19');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('20', '1', '20');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('21', '1', '21');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('22', '1', '22');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('23', '1', '23');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('24', '1', '24');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('25', '1', '25');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('26', '1', '26');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('27', '1', '27');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('28', '1', '28');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('29', '1', '29');
INSERT INTO "acl" ("id", "roleid", "carteid") VALUES ('30', '1', '30');