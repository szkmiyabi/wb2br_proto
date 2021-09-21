using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wb2br_proto
{
    public class PresvUtil
    {
        public static string css_cut()
        {
            return @"
                var d = document;
                var delarr = new Array();
                var links = d.getElementsByTagName(""link"");
                for(var i=0; i<links.length; i++) {
                    var link = links.item(i);
                    var href = link.getAttribute(""href"");
                    if(is_css_file(href) || is_css_link(link)) {
                        delarr.push(href);
                    }
                }
                for(var i=0; i<delarr.length; i++) {
                    var line = delarr[i];
                    delete_link(line);
                }
                var tags = d.getElementsByTagName(""*"");
                for(var i=0; i<tags.length; i++) {
                    var tag = tags.item(i);
                    var style = tag.getAttribute(""style"");
                    if(style !== null || style !== """") {
                        tag.removeAttribute(""style"");
                    }
                }
                var styles = d.getElementsByTagName(""style"");
                for(i=0; i<styles.length; i++) {
                    var style = styles.item(i);
                    style.textContent = null;
                }
                function is_css_file(href) {
                    var pat = new RegExp("".+\.css"");
                    if(pat.test(href)) return true;
                    else return false;
                }
                function is_css_link(link) {
                    if(link.hasAttribute(""type"")) {
                        var pat = new RegExp(""text/css"");
                        var type = link.getAttribute(""type"");
                        if(pat.test(type)) return true;
                        else return false;
                    } else {
                        return false;
                    }
                }
                function delete_link(line) {
                    var lks = d.getElementsByTagName(""link"");
                    for(var j=0; j<lks.length; j++) {
                        var lk = lks.item(j);
                        var hf = lk.getAttribute(""href"");
                        if(hf === line) {
                            lk.parentNode.removeChild(lk);
                            break;
                        }
                    }
                }
            ";
        }

        public static string image_alt()
        {
            return @"
                var fname_flg = true;
                var img = document.getElementsByTagName(""img"");
                for(var i=0; i<img.length; i++) {
                    var imgtag = img.item(i);
                    imgtag.setAttribute(""style"", ""border:1px solid red;"");
                    var span_id = ""bkm-img-span-"" + i;
                    var src_val = imgtag.getAttribute(""src"");
                    var fname = get_img_filename(src_val);
                    var alt_val = imgtag.getAttribute(""alt"");
                    if(alt_val === null) {
                        alt_val = alt_attr_from_dirtycode(imgtag);
                    }
                    var html_str = """";
                    if(alt_attr_check(imgtag)) {
                        html_str += ""alt: "" + alt_val;
                    } else {
                        html_str += ""alt属性がない"";
                    }
                    if(fname_flg) {
                        if(html_str !== """") {
                            html_str += "", filename: "" + fname;
                        } else {
                            html_str += ""filename: "" + fname;
                        }
                    }
                    var css_txt = ""color:#fff;font-size:12px;padding:1px;background:#BF0000;"";
                    var span = '<span id=""' + span_id + '"" style=""' + css_txt + '"">' + html_str + '</span>';
                    imgtag.insertAdjacentHTML(""beforebegin"", span);
                }
                tag_link_img();
                tag_area();
                function alt_attr_from_dirtycode(obj) {
                    var ret = """";
                    var imgtag = obj.outerHTML;
                    var pt = new RegExp('(alt="")(.*?)("")');
                    if(pt.test(imgtag)) {
                        ret = imgtag.match(pt)[2];
                    }
                    return ret;
                }
                function get_img_filename(str) {
                    var ret = """";
                    var pat = new RegExp(""(.+)\/(.+\.)(JPG|jpg|GIF|gif|PNG|png|BMP|bmp)$"");
                    if(pat.test(str)) {
                        var arr = str.match(pat);
                        ret += arr[2] + arr[3];
                    }
                    return ret;
                }
                function alt_attr_check(imgtag) {
                    var txt = imgtag.outerHTML;
                    var pt1 = new RegExp('alt="".*""');
                    var pt2 = new RegExp('alt=');
                    if(pt1.test(txt) && pt2.test(txt)) return true;
                    else return false;
                }
                function tag_link_img() {
                    var ats = document.getElementsByTagName(""a"");
                    var css_txt = ""border:2px dotted red;"";
                    for(var i=0; i<ats.length; i++) {
                        var atag = ats.item(i);
                        var imgs = atag.getElementsByTagName(""img"");
                        for(var j=0; j<imgs.length; j++) {
                            var img = imgs.item(j);
                            img.setAttribute(""style"", css_txt);
                        }
                    }
                }
                function tag_area() {
                    var area = document.getElementsByTagName(""area"");
                    for(var i=0; i<area.length; i++) {
                        var areatag = area.item(i);
                        var href_val = areatag.getAttribute(""href"");
                        var alt_val = areatag.getAttribute(""alt"");
                        var css_txt = ""color:#fff;font-size:12px;padding:1px;background:#7B0D0D;margin-right:2px;display:inline-block;border: 2px dotted #fdcf1b;"";
                        var html_str = """";
                        if(_alt_attr_check(areatag)) {
                            html_str += ""alt: "" + alt_val;
                        } else {
                            html_str += ""alt属性がない"";
                        }
                        html_str += "", url: "" + href_val;
                        var target_val = """";
                        if(areatag.hasAttribute(""target"")) {
                            target_val = areatag.getAttribute(""target"");
                        } else {
                            target_val = null;
                        }
                        if(target_val != null) {
                            if(target_val == """") {
                                html_str += `, <span style=""background-color:#0f7136 !important;margin:2px;"">target属性有:(空)</span>`;
                            } else {
                                html_str += `, <span style=""background-color:#0f7136 !important;margin:2px;"">target属性有:${target_val}</span>`;
                            }
                        }
                        var span = `<span id=""bkm-area-span-${i}"" style=""${css_txt}"">area要素, ` + html_str + ""</span>"";
                        areatag.insertAdjacentHTML(""beforebegin"", span);
                    }
                    function _alt_attr_check(areatag) {
                        var txt = areatag.outerHTML;
                        var pt1 = new RegExp('alt="".*""');
                        var pt2 = new RegExp('alt=');
                        if(pt1.test(txt) && pt2.test(txt)) return true;
                        else return false;
                    }
                }
            ";
        }

        public static string target_attr()
        {
            return @"
                var ats = document.getElementsByTagName(""a"");
                for(var i=0; i<ats.length; i++) {
                    var atag = ats.item(i);
                    var ataghtml = atag.outerHTML;
                    ataghtml = _text_clean(ataghtml);
                    if(_target_attr_check(ataghtml)) {
                        var target_vl = atag.getAttribute(""target"");
                        var span_id = ""bkm-target-attr-span-"" + i;
                        var span_html = (target_vl === """") ? ""target属性有:(空)"" : ""target属性有:"" + target_vl;
                        var span_css = ""padding-right:5px;color:#fff;font-size:12px;padding:1px;background:#008000;border-radius:5px;"";
                        var span = '<span id=""' + span_id + '"" style=""' + span_css + '"">' + span_html + '</span>';
                        atag.insertAdjacentHTML(""beforebegin"", span);
                    }
                }
                function _target_attr_check(str) {
                    var pt = new RegExp('target="".*?""');
                    if(pt.test(str)) return true;
                    else return false;
                }
                function _text_clean(str) {
                    var ret = """";
                    ret = str.replace(new RegExp(""^ +"", ""mg""), """");
                    ret = ret.replace(new RegExp(""\\t+"", ""mg""), """");
                    ret = ret.replace(new RegExp(""(\\r\\n|\\r|\\n)"", ""mg""), """");
                    return str;
                }
            ";
        }
    }
}
