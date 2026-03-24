// ESC键关闭图层
if (!window.loginLayerEscBound) {
    $(document).on('keydown', function (e) {
        if (e.keyCode === 27 && window.layerEscIndex?.length) {
            layer.close(window.layerEscIndex[0]);
        }
    });
    window.loginLayerEscBound = true;
}

// SM3加密函数
function sm3Encrypt(data) {
    try {
        const msgData = CryptoJS.enc.Utf8.parse(data);
        const sm3keycur = new SM3Digest();
        const words = sm3keycur.GetWords(msgData.toString());
        sm3keycur.BlockUpdate(words, 0, words.length);
        const c3 = new Array(32);
        sm3keycur.DoFinal(c3, 0);
        return sm3keycur.GetHex(c3).toString().toLowerCase();
    } catch (e) {
        console.error('SM3加密失败:', e);
        return data;
    }
}

// POST请求函数
async function postData(url, data) {
    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: data
        });

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }

        return await response.json();
    } catch (error) {
        console.error('请求失败:', error);
        throw error;
    }
}



// ================== 辅助函数：用于 onclick 参数转义（使用 JSON.stringify 保证安全）==================
function escapeForOnClick(str) {
    return JSON.stringify(str); // 返回带引号的字符串字面量，如 "测试\"内容"
}

// ================== 过滤导航项 ==================
function filterNavs(keyword) {
    if (!keyword.trim()) return ALL_NAVS;  // 空关键词返回全部

    const lowerKeyword = keyword.trim().toLowerCase();
    return ALL_NAVS.filter(item => {
        return (item.NavName && item.NavName.toLowerCase().includes(lowerKeyword)) ||
            (item.NavLink && item.NavLink.toLowerCase().includes(lowerKeyword));
    });
}

// ================== 按部门分组（保持部门顺序）==================
function groupByDepartment(filteredNavs) {
    const groups = [];
    DEPARTMENTS.forEach(dep => {
        const items = filteredNavs.filter(n => n.DepName === dep.value);
        if (items.length > 0) {
            // 按 OrderBy 排序
            items.sort((a, b) => (a.OrderBy || 0) - (b.OrderBy || 0));
            groups.push({
                depName: dep.name,
                items: items
            });
        }
    });
    return groups;
}

// ================== 渲染导航列表 ==================
function renderNavs(groups) {
    const container = document.getElementById('navContainer');
    const searchInfo = document.getElementById('searchInfo');
    const keyword = document.getElementById('searchInput').value.trim();

    if (groups.length === 0) {
        // 显示空状态（无匹配结果）
        let emptyHtml = `
                <div class="empty-state">
                    <i class="fas fa-map-signs"></i>
                    <h3>${keyword ? '没有找到匹配的导航' : '暂无导航数据'}</h3>
                    <p>${keyword ? '试试其他关键词吧' : '还没有添加任何导航链接'}</p>
            `;

        emptyHtml += `</div>`;
        container.innerHTML = emptyHtml;
        searchInfo.innerText = '';
        return;
    }

    // 更新搜索提示信息
    const totalCount = groups.reduce((acc, g) => acc + g.items.length, 0);
    searchInfo.innerText = keyword ? `找到 ${totalCount} 个匹配的导航` : '';

    // 构建 HTML
    let html = '';
    groups.forEach(group => {
        const depName = escapeHtml(group.depName);
        const itemCount = group.items.length;

        html += `
                <div class="nav-department-row animate__animated">
                    <div class="dep-header">
                        <div class="dep-title">
                            <i class="fas fa-folder"></i>
                            ${depName}
                        </div>
                        <div class="dep-count">
                            <i class="fas fa-link"></i> 共 ${itemCount} 个导航
                        </div>
                    </div>
                    <div class="nav-links-container">
            `;

        group.items.forEach(item => {
            // 使用 escapeHtml 处理文本内容（防止 XSS）
            const id = item.Id;
            const navName = escapeHtml(item.NavName);
            const navLink = escapeHtml(item.NavLink);
            const shortLink = navLink.length > 35 ? navLink.substring(0, 34) + '...' : navLink;
            const isCreator = (item.Creater === CURRENT_USER);

            html += `
                    <div class="nav-item-card">
                        <div class="nav-content">
                            <a href="${navLink}" target="_blank" title="${navLink}">
                                <span class="nav-name">${navName}</span>
                                <span class="nav-url">${shortLink}</span>
                            </a>
                        </div>
                `;

            if (isCreator) {
                html += `
                        <div class="action-buttons">
                            <a href="#" onclick="EditNav('${id}', '${navName}', '${navLink}', '${item.DepName}', '1')" class="edit-btn" title="编辑">
                                <i class="fas fa-edit"></i>
                            </a>
                            <a href="#" onclick="deleteNav('${id}')" class="delete-btn" title="删除">
                                <i class="fas fa-trash-alt"></i>
                            </a>
                        </div>
                    `;
            }

            html += `</div>`;
        });

        html += `</div></div>`;
    });

    container.innerHTML = html;
}

// 简单的 HTML 转义函数（用于显示内容）
function escapeHtml(unsafe) {
    if (!unsafe) return '';
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

// ================== 搜索处理（防抖）==================
let searchTimer;
function handleSearch() {
    clearTimeout(searchTimer);
    const input = document.getElementById('searchInput');
    const clearBtn = document.getElementById('clearSearch');
    const keyword = input.value;

    // 显示/隐藏清空按钮
    clearBtn.style.display = keyword ? 'block' : 'none';

    searchTimer = setTimeout(() => {
        const filtered = filterNavs(keyword);
        const groups = groupByDepartment(filtered);
        renderNavs(groups);
    }, 300); // 300ms 防抖
}

// ================== 清空搜索 ==================
function clearSearch() {
    const input = document.getElementById('searchInput');
    input.value = '';
    document.getElementById('clearSearch').style.display = 'none';
    handleSearch(); // 直接触发刷新
}

// ================== 初始化 ==================
document.addEventListener('DOMContentLoaded', function () {
    // 绑定搜索事件
    const searchInput = document.getElementById('searchInput');
    const clearBtn = document.getElementById('clearSearch');
    if (searchInput) {
        searchInput.addEventListener('input', handleSearch);
        clearBtn.addEventListener('click', clearSearch);
    }

    // 初始渲染：基于 ALL_NAVS 渲染，覆盖静态内容以保证排序和一致性
    const initialGroups = groupByDepartment(ALL_NAVS);
    renderNavs(initialGroups);
});


function AddNav() {
    // 构建部门下拉选项
    let depOptions = '';
    DEPARTMENTS.forEach(dep => {
        depOptions += `<option value="${dep.value}">${dep.name}</option>`;
    });

    layer.open({
        title: '新增导航',
        area: ['480px', '405px'],
        shade: 0.3,
        anim: 0,
        type: 1,
        content: `
                <div style="padding: 25px; box-sizing: border-box;">
                    <form id="addForm" onsubmit="return false;">
                        <div class="layui-form-item">
                            <label class="layui-form-label">导航名称</label>
                            <div class="layui-input-block">
                                <input type="text" id="addNavName" required placeholder="请输入导航名称" class="layui-input">
                            </div>
                        </div>
                        <div class="layui-form-item">
                            <label class="layui-form-label">导航链接</label>
                            <div class="layui-input-block">
                            <!-- 改为多行文本框，支持换行输入 -->
                            <textarea
                                id="addNavLink"
                                required
                                placeholder="请输入导航链接（如 https://xxx.com），每行一个链接"
                                class="layui-textarea"
                                style="min-height: 60px; resize: vertical;"
                                rows="4"></textarea>                           
                            </div>                   
                        </div>
                        <div class="layui-form-item">
                            <label class="layui-form-label">部门名称</label>
                            <div class="layui-input-block">
                                <select id="addDepName" required class="layui-select">
                                    <option value="">请选择部门</option>
                                    ${depOptions}
                                </select>
                            </div>
                        </div>
                        <div id="addMsg" style="color:var(--danger-color);text-align:center;min-height:20px; margin-top:15px;"></div>
                    </form>
                </div>
            `,
        btn: [' 确认新增 ', ' 取消 '],
        yes: function (index, layero) {
            const navName = layero.find('#addNavName').val().trim();
            const navLink = layero.find('#addNavLink').val().trim();
            const depName = layero.find('#addDepName').val().trim();
            const orderBy = "1";
            const $msg = layero.find('#addMsg');

            if (!navName) {
                $msg.text('请输入导航名称');
                layero.find('#addNavName').focus();
                return;
            }
            if (!navLink) {
                $msg.text('请输入导航链接');
                layero.find('#addNavLink').focus();
                return;
            }
            try {
                new URL(navLink);
            } catch (e) {
                $msg.text('请输入有效的URL地址（需包含 http/https）');
                layero.find('#addNavLink').focus();
                return;
            }
            if (!depName) {
                $msg.text('请选择部门名称');
                layero.find('#addDepName').focus();
                return;
            }

            $msg.html('<i class="custom-loading" style="display:inline-block; width:20px; height:20px; border:2px solid #fcd; border-top-color:#ffa; border-radius:50%; animation:spin 1s linear infinite;"></i> 正在保存...');

            const navData = JSON.stringify({
                NavName: navName,
                NavLink: navLink,
                DepName: depName,
                Creater: CURRENT_USER,
                OrderBy: parseInt(orderBy)
            });
            const encryptedHash = sm3Encrypt(navData);

            fetch('/Nav/AddNav/' + encryptedHash, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                },
                body: navData
            })
                .then(response => response.json())
                .then(result => {
                    if (result.code === 1 || result.success === true) {
                        layer.close(index);
                        layer.msg('✅  新增成功！', { icon: 1, time: 1200 }, function () {
                            refreshCache();
                        });
                    } else {
                        $msg.text(result.message || '新增失败，请重试');
                    }
                })
                .catch(error => {
                    console.error('新增失败:', error);
                    $msg.text('新增失败，请稍后重试');
                });
        },
        btn2: function (index) {
            layer.close(index);
        },
        success: function (layero) {
            layer.setTop?.(layero);
            layero.find('#addNavName').focus();
        }
    });
}

function LoginLayer() {
    layer.open({
        title: '管理登录',
        area: ['390px', '290px'],
        shade: 0.3,
        anim: 0,
        type: 1,
        content: `
                <div style="padding:10px 30px 5px 20px;box-sizing:border-box;">
                    <form id="loginForm" onsubmit="return false;">
                        <div class="layui-form-item">
                            <label class="layui-form-label" style="width:70px;padding-left:0;">用户名</label>
                            <div class="layui-input-block" style="margin-left:80px;">
                                <input type="text" name="username" required placeholder="请输入用户名"
                                        autocomplete="username" class="layui-input">
                            </div>
                        </div>
                        <div class="layui-form-item">
                            <label class="layui-form-label" style="width:70px;padding-left:0;">密码</label>
                            <div class="layui-input-block" style="margin-left:80px;">
                                <input type="password" name="password" required placeholder="请输入密码"
                                        autocomplete="current-password" class="layui-input">
                            </div>
                        </div>
                        <div id="loginMsg" style="color:#ff5722;text-align:center;min-height:20px;"></div>
                    </form>
                </div>
            `,
        btn: [' 登录 ', ' 取消 '],
        yes: async function (index, layero) {
            const $form = layero.find('#loginForm');
            const username = $form.find('[name="username"]').val().trim();
            const password = $form.find('[name="password"]').val().trim();
            const $msg = layero.find('#loginMsg');

            if (!username) return showMsg('请输入用户名', '[name="username"]');
            if (password.length < 6) return showMsg('密码至少6位', '[name="password"]');

            $msg.html('<i class="custom-loading" style="display:inline-block; width:20px; height:20px; border:2px solid #fcd; border-top-color:#ffa; border-radius:50%; animation:spin 1s linear infinite;"></i>   正在登录...');

            try {
                const loginData = { UserName: username, PassWord: password };
                const jsonData = JSON.stringify(loginData);
                const encryptedHash = sm3Encrypt(jsonData);

                const response = await postData('/API/MgLogin/' + encryptedHash, jsonData);

                if (response.code == 1) {
                    layer.close(index);
                    layer.msg('✅    登录成功！', { icon: 1, time: 1200 }, async function () {
                        try {
                            await $.get('/SetLoginUser/' + username);
                            location.reload();
                        } catch (e) {
                            console.error('设置登录状态失败:', e);
                            location.reload();
                        }
                    });
                } else {
                    showError(response.message || '用户名或密码错误');
                }
            } catch (error) {
                console.error('登录请求失败:', error);
                showError('登录失败，请稍后重试');
            }

            function showMsg(text, focusSelector) {
                $msg.text(text);
                if (focusSelector) $form.find(focusSelector).focus();
            }

            function showError(text) {
                $msg.text(text);
                $form.find('[name="password"]').val('').focus();
            }
        },
        btn2: function (index) {
            layer.close(index);
        },
        zIndex: layer.zIndex,
        success: function (layero) {
            layer.setTop?.(layero);
            window.layerEscIndex = window.layerEscIndex || [];
            window.layerEscIndex.unshift(layero.attr('times'));
            layero.on('mousedown', () => {
                const idx = layero.attr('times');
                const pos = window.layerEscIndex.indexOf(idx);
                if (pos > -1) window.layerEscIndex.splice(pos, 1);
                window.layerEscIndex.unshift(idx);
            });
            layero.find('[name="username"]').focus();

            // ===== 新增：回车登录 =====
            layero.find('input[name="username"], input[name="password"]').on('keydown', function (e) {
                if (e.keyCode === 13) {
                    e.preventDefault(); // 阻止默认行为（如表单提交）
                    layero.find('.layui-layer-btn0').click(); // 触发登录按钮点击
                }
            });
            // ===== 结束 =====
        },
        end: function () {
            if (Array.isArray(window.layerEscIndex)) window.layerEscIndex.shift();
        }
    });
}

function logout(outuser) {
    layer.confirm(' 确定要退出登录吗？', {
        title: '退出确认',
        icon: 3,
        btn: [' 确定退出 ', ' 取消 ']
    }, function (index) {
        layer.close(index);
        layer.msg('正在退出...', { icon: 16, shade: 0.3, time: 800 }, function () {
            $.get('/API/LogOut/' + outuser, function () {
                location.reload();
            }).always(function () {
                location.reload();
            });
        });
    });
}

function refreshCache() {
    window.location.href = '?refresh=true';
}

function EditNav(id, navName, navLink, depName) {
    // 构建部门下拉选项并设置默认选中
    let depOptions = '';
    DEPARTMENTS.forEach(dep => {
        const isSelected = dep.value === depName ? 'selected' : '';
        depOptions += `<option value="${dep.value}" ${isSelected}>${dep.name}</option>`;
    });

    layer.open({
        title: '编辑导航',
        area: ['450px', '378px'],
        shade: 0.3,
        anim: 0,
        type: 1,
        content: `
                <div style="padding:20px 30px 5px 20px;box-sizing:border-box;">
                    <form id="editForm" onsubmit="return false;">
                        <input type="hidden" id="navId" value="${id}">
                        <div class="layui-form-item">
                            <label class="layui-form-label" style="width:70px;padding-left:0;">导航名称</label>
                            <div class="layui-input-block" style="margin-left:80px;">
                                <input type="text" id="editNavName" value="${navName.replace(/'/g, "\\'")}" required placeholder="请输入导航名称" class="layui-input">
                            </div>
                        </div>
                        <div class="layui-form-item">
                            <label class="layui-form-label" style="width:70px;padding-left:0;">导航链接</label>
                            <div class="layui-input-block" style="margin-left:80px;">
                                <input type="url" id="editNavLink" value="${navLink.replace(/'/g, "\\'")}" required placeholder="请输入导航链接" class="layui-input">
                            </div>
                        </div>
                        <div class="layui-form-item">
                            <label class="layui-form-label" style="width:70px;padding-left:0;">部门名称</label>
                            <div class="layui-input-block" style="margin-left:80px;">
                                <select id="editDepName" required class="layui-select">
                                    <option value="">请选择部门</option>
                                    ${depOptions}
                                </select>
                            </div>
                        </div>
                        <div id="editMsg" style="color:#ff5722;text-align:center;min-height:20px; margin-top:10px;"></div>
                    </form>
                </div>
            `,
        btn: [' 保存修改 ', ' 取消 '],
        yes: function (index, layero) {
            const id = layero.find('#navId').val();
            const navName = layero.find('#editNavName').val().trim();
            const navLink = layero.find('#editNavLink').val().trim();
            const depName = layero.find('#editDepName').val().trim();
            const orderBy = "1";
            const $msg = layero.find('#editMsg');

            if (!navName) {
                $msg.text('请输入导航名称');
                layero.find('#editNavName').focus();
                return;
            }
            if (!navLink) {
                $msg.text('请输入导航链接');
                layero.find('#editNavLink').focus();
                return;
            }
            try {
                new URL(navLink);
            } catch (e) {
                $msg.text('请输入有效的URL地址');
                layero.find('#editNavLink').focus();
                return;
            }
            if (!depName) {
                $msg.text('请选择部门名称');
                layero.find('#editDepName').focus();
                return;
            }

            $msg.html('<i class="custom-loading" style="display:inline-block; width:20px; height:20px; border:2px solid #fcd; border-top-color:#ffa; border-radius:50%; animation:spin 1s linear infinite;"></i>  正在保存...');

            const navData = JSON.stringify({
                Id: id,
                NavName: navName,
                NavLink: navLink,
                DepName: depName,
                OrderBy: parseInt(orderBy)
            });
            const encryptedHash = sm3Encrypt(navData);

            fetch('/Nav/EditNav/' + encryptedHash, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                },
                body: navData
            })
                .then(response => response.json())
                .then(result => {
                    if (result.code === 1 || result.success === true) {
                        layer.close(index);
                        layer.msg('✅ 编辑成功！', { icon: 1, time: 1200 }, function () {
                            refreshCache();
                        });
                    } else {
                        $msg.text(result.message || '编辑失败，请重试');
                    }
                })
                .catch(error => {
                    console.error('编辑失败:', error);
                    $msg.text('编辑失败，请稍后重试');
                });
        },
        btn2: function (index) {
            layer.close(index);
        },
        success: function (layero) {
            layer.setTop?.(layero);
            layero.find('#editNavName').focus();
        }
    });
}

function deleteNav(id) {
    layer.confirm('确定要删除这个导航吗？此操作不可恢复。', {
        title: '删除确认',
        icon: 3,
        btn: [' 确定删除 ', ' 取消 ']
    }, function (index) {
        layer.close(index);
        layer.msg('正在删除...', { icon: 16, shade: 0.3, time: 800 });

        const navData = JSON.stringify({ Id: id });
        const encryptedHash = sm3Encrypt(navData);

        fetch('/Nav/DelNav/' + encryptedHash, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            },
            body: navData
        })
            .then(response => response.json())
            .then(result => {
                if (result.code === 1 || result.success === true) {
                    layer.msg('✅ 删除成功！', { icon: 1, time: 1200 }, function () {
                        refreshCache();
                    });
                } else {
                    layer.msg(result.message || '删除失败，请重试', { icon: 2 });
                }
            })
            .catch(error => {
                console.error('删除失败:', error);
                layer.msg('删除失败，请稍后重试', { icon: 2 });
            });
    });
}

async function postData(url = '', data = '') {
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
        },
        body: data
    });
    return response.json();
}


                (function() {
        // 搜索框防抖延迟（毫秒）
        const SEARCH_DEBOUNCE_DELAY = 300;
        let debounceTimer = null;
    
        // 获取元素
        const searchInput = document.getElementById('searchInput');
        const clearBtn = document.getElementById('clearSearch');
    
        if (!searchInput) return;
    
        // 更新清除按钮可见性
        function updateClearButtonVisibility() {
            if (clearBtn) {
                const hasValue = searchInput.value.trim().length > 0;
                if (hasValue) {
                    clearBtn.classList.add('visible');
                } else {
                    clearBtn.classList.remove('visible');
                }
            }
        }
    
        // 触发搜索（可扩展为全局事件或回调）
        function triggerSearch(keyword) {
            // 方法1：派发自定义事件，供页面具体业务监听
            const searchEvent = new CustomEvent('navSearch', {
                detail: { keyword: keyword, timestamp: Date.now() },
                bubbles: true,
                cancelable: true
            });
            document.dispatchEvent(searchEvent);        
        
        }
    
        // 防抖搜索
        function debouncedSearch() {
            if (debounceTimer) clearTimeout(debounceTimer);
            debounceTimer = setTimeout(() => {
                const keyword = searchInput.value.trim();
                triggerSearch(keyword);
            }, SEARCH_DEBOUNCE_DELAY);
        }
    
        // 立即搜索（用于回车和清空操作）
        function immediateSearch() {
            if (debounceTimer) clearTimeout(debounceTimer);
            const keyword = searchInput.value.trim();
            triggerSearch(keyword);
        }
    
        // 清空输入框
        function clearSearchInput() {
            if (searchInput.value === '') return;
            searchInput.value = '';
            updateClearButtonVisibility();
            immediateSearch(); // 清空后立即触发搜索，重置结果
        
            searchInput.focus();
        }
    
        // 绑定事件
        if (searchInput) {
            // 输入事件（带防抖）
            searchInput.addEventListener('input', function(e) {
                updateClearButtonVisibility();
                debouncedSearch();
            });
        
            // 键盘事件 - 回车立即搜索
            searchInput.addEventListener('keydown', function(e) {
                if (e.key === 'Enter') {
                    e.preventDefault();
                    if (debounceTimer) clearTimeout(debounceTimer);
                    immediateSearch();
                }
                // ESC 键清空并收起结果
                if (e.key === 'Escape') {
                    if (searchInput.value.trim() !== '') {
                        clearSearchInput();
                        e.preventDefault();
                    } else {
                        searchInput.blur();
                    }
                }
            });
        
            // 初始化清除按钮可见性
            updateClearButtonVisibility();
        }
    
        if (clearBtn) {
            clearBtn.addEventListener('click', function(e) {
                e.preventDefault();
                clearSearchInput();
            });
        }
        window.resetNavSearch = function() {
            if (searchInput) {
                searchInput.value = '';
                updateClearButtonVisibility();
                triggerSearch('');
            }
        };
    })();