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
    }
}
