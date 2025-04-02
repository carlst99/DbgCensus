import{_ as i,c as a,o as n,ae as e}from"./chunks/framework.DHm_ITtO.js";const o=JSON.parse('{"title":"Setup","description":"","frontmatter":{},"headers":[],"relativePath":"rest/index.md","filePath":"rest/index.md"}'),l={name:"rest/index.md"};function t(p,s,h,k,r,d){return n(),a("div",null,s[0]||(s[0]=[e(`<h1 id="setup" tabindex="-1">Setup <a class="header-anchor" href="#setup" aria-label="Permalink to “Setup”">​</a></h1><p>This document will guide you through installing and setting up <code>DbgCensus.Rest</code>, the package for interacting with Census&#39; query interface.</p><div class="tip custom-block"><p class="custom-block-title">TIP</p><p>Check out the <a href="https://github.com/carlst99/DbgCensus/tree/main/Samples/RestSample" target="_blank" rel="noreferrer">REST Sample</a> as you read through this guide.</p><p>It&#39;s recommended that you use a template that wires up the <a href="https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host" target="_blank" rel="noreferrer">Generic Host</a>, such as a <em>Worker Service</em> or an <em>ASP.NET Core</em> project.</p></div><div class="info custom-block"><p class="custom-block-title">INFO</p><p><code>DbgCensus.Rest</code> configures a wait-and-retry <a href="https://github.com/App-vNext/Polly" target="_blank" rel="noreferrer">Polly</a> policies by default. This will perform a jittered exponential backoff up to four (by default) times when a query fails.</p><p>See <a href="./advanced.html">Advanced Configuration</a> for more information.</p></div><ol><li><p>Install <code>DbgCensus.Rest</code>:</p><div class="language-powershell vp-adaptive-theme line-numbers-mode"><button title="Copy Code" class="copy"></button><span class="lang">powershell</span><pre class="shiki shiki-themes one-dark-pro one-light vp-code" tabindex="0"><code><span class="line"><span style="--shiki-dark:#7F848E;--shiki-dark-font-style:italic;--shiki-light:#A0A1A7;--shiki-light-font-style:italic;"># Visual Studio Package Manager</span></span>
<span class="line"><span style="--shiki-dark:#56B6C2;--shiki-light:#0184BC;">Install-Package</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;"> DbgCensus.Rest</span></span>
<span class="line"><span style="--shiki-dark:#7F848E;--shiki-dark-font-style:italic;--shiki-light:#A0A1A7;--shiki-light-font-style:italic;"># dotnet console</span></span>
<span class="line"><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">dotnet add package DbgCensus.Rest</span></span></code></pre><div class="line-numbers-wrapper" aria-hidden="true"><span class="line-number">1</span><br><span class="line-number">2</span><br><span class="line-number">3</span><br><span class="line-number">4</span><br></div></div></li><li><p>Register the required types to an <code>IServiceCollection</code> with the <code>AddCensusRestServices</code> extension method.<br> If you aren&#39;t using <code>Microsoft.Extensions.DependencyInjection</code>, take a look at <a href="https://github.com/carlst99/DbgCensus/blob/main/DbgCensus.Rest/Extensions/IServiceCollectionExtensions.cs" target="_blank" rel="noreferrer">this file</a> to see how the required services are setup.</p></li><li><p>Configure an instance of the <code>CensusQueryOptions</code> class to ensure that your service ID is utilised.<br> Typically, you&#39;d register your options from a configuration source (such as a section of <code>appsettings.json</code>) to retrieve any secrets that shouldn&#39;t be stored with the code (i.e. - the service ID!), and then follow up with any additional runtime configuration.</p></li></ol><p><strong>Example</strong></p><div class="language-csharp vp-adaptive-theme line-numbers-mode"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes one-dark-pro one-light vp-code" tabindex="0"><code><span class="line"><span style="--shiki-dark:#C678DD;--shiki-light:#A626A4;">using</span><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;"> DbgCensus</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">.</span><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;">Rest</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">;</span></span>
<span class="line"><span style="--shiki-dark:#C678DD;--shiki-light:#A626A4;">using</span><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;"> DbgCensus</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">.</span><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;">Rest</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">.</span><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;">Extensions</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">;</span></span>
<span class="line"><span style="--shiki-dark:#C678DD;--shiki-light:#A626A4;">using</span><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;"> Microsoft</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">.</span><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;">Extensions</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">.</span><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;">DependencyInjection</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">;</span></span>
<span class="line"><span style="--shiki-dark:#C678DD;--shiki-light:#A626A4;">using</span><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;"> Microsoft</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">.</span><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;">Extensions</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">.</span><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;">Hosting</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">;</span></span>
<span class="line"><span style="--shiki-dark:#C678DD;--shiki-light:#A626A4;">using</span><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;"> System</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">.</span><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;">Threading</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">.</span><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;">Tasks</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">;</span></span>
<span class="line"></span>
<span class="line"><span style="--shiki-dark:#C678DD;--shiki-light:#A626A4;">namespace</span><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;"> RestSample</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">;</span></span>
<span class="line"></span>
<span class="line"><span style="--shiki-dark:#C678DD;--shiki-light:#A626A4;">public</span><span style="--shiki-dark:#C678DD;--shiki-light:#A626A4;"> static</span><span style="--shiki-dark:#C678DD;--shiki-light:#A626A4;"> class</span><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;"> Program</span></span>
<span class="line"><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">{</span></span>
<span class="line"><span style="--shiki-dark:#C678DD;--shiki-light:#A626A4;">    public</span><span style="--shiki-dark:#C678DD;--shiki-light:#A626A4;"> static</span><span style="--shiki-dark:#C678DD;--shiki-light:#A626A4;"> async</span><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;"> Task</span><span style="--shiki-dark:#61AFEF;--shiki-light:#4078F2;"> Main</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">(</span><span style="--shiki-dark:#C678DD;--shiki-light:#A626A4;">string</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">[] </span><span style="--shiki-dark:#E5C07B;--shiki-light:#383A42;">args</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">)</span></span>
<span class="line"><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">    {</span></span>
<span class="line"><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;">        IHost</span><span style="--shiki-dark:#E06C75;--shiki-light:#383A42;"> host</span><span style="--shiki-dark:#56B6C2;--shiki-light:#383A42;"> =</span><span style="--shiki-dark:#E5C07B;--shiki-light:#383A42;"> Host</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">.</span><span style="--shiki-dark:#61AFEF;--shiki-light:#4078F2;">CreateDefaultBuilder</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">(</span><span style="--shiki-dark:#E06C75;--shiki-light:#383A42;">args</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">)</span></span>
<span class="line"><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">            .</span><span style="--shiki-dark:#61AFEF;--shiki-light:#4078F2;">ConfigureServices</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">((</span><span style="--shiki-dark:#E5C07B;--shiki-light:#383A42;">hostContext</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">, </span><span style="--shiki-dark:#E5C07B;--shiki-light:#383A42;">services</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">) =&gt;</span></span>
<span class="line"><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">            {</span></span>
<span class="line highlighted"><span style="--shiki-dark:#E5C07B;--shiki-light:#383A42;">                services</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">.</span><span style="--shiki-dark:#61AFEF;--shiki-light:#4078F2;">Configure</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">&lt;</span><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;">CensusQueryOptions</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">&gt;(</span><span style="--shiki-dark:#E5C07B;--shiki-light:#383A42;">hostContext</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">.</span><span style="--shiki-dark:#E5C07B;--shiki-light:#383A42;">Configuration</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">.</span><span style="--shiki-dark:#61AFEF;--shiki-light:#4078F2;">GetSection</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">(nameof(</span><span style="--shiki-dark:#E06C75;--shiki-light:#383A42;">CensusQueryOptions</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">)));</span></span>
<span class="line"></span>
<span class="line"><span style="--shiki-dark:#7F848E;--shiki-dark-font-style:italic;--shiki-light:#A0A1A7;--shiki-light-font-style:italic;">                // AND/OR</span></span>
<span class="line"><span style="--shiki-dark:#E5C07B;--shiki-light:#383A42;">                services</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">.</span><span style="--shiki-dark:#61AFEF;--shiki-light:#4078F2;">Configure</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">&lt;</span><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;">CensusQueryOptions</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">&gt;(</span><span style="--shiki-dark:#E5C07B;--shiki-light:#383A42;">o</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;"> =&gt;</span></span>
<span class="line"><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">                {</span></span>
<span class="line"><span style="--shiki-dark:#E5C07B;--shiki-light:#383A42;">                    o</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">.</span><span style="--shiki-dark:#E5C07B;--shiki-light:#383A42;">LanguageCode</span><span style="--shiki-dark:#56B6C2;--shiki-light:#383A42;"> =</span><span style="--shiki-dark:#E5C07B;--shiki-light:#383A42;"> CensusLanguage</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">.</span><span style="--shiki-dark:#E5C07B;--shiki-light:#383A42;">English</span></span>
<span class="line"><span style="--shiki-dark:#7F848E;--shiki-dark-font-style:italic;--shiki-light:#A0A1A7;--shiki-light-font-style:italic;">                    // Etc.</span></span>
<span class="line"><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">                });</span></span>
<span class="line"></span>
<span class="line highlighted"><span style="--shiki-dark:#E5C07B;--shiki-light:#383A42;">                services</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">.</span><span style="--shiki-dark:#61AFEF;--shiki-light:#4078F2;">AddCensusRestServices</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">();</span></span>
<span class="line"><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">                ...</span></span>
<span class="line"><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">            })</span></span>
<span class="line"><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">            .</span><span style="--shiki-dark:#61AFEF;--shiki-light:#4078F2;">Build</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">();</span></span>
<span class="line"></span>
<span class="line"><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">        await </span><span style="--shiki-dark:#E5C07B;--shiki-light:#383A42;">host</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">.</span><span style="--shiki-dark:#61AFEF;--shiki-light:#4078F2;">RunAsync</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">();</span></span>
<span class="line"><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">    }</span></span>
<span class="line"><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">}</span></span></code></pre><div class="line-numbers-wrapper" aria-hidden="true"><span class="line-number">1</span><br><span class="line-number">2</span><br><span class="line-number">3</span><br><span class="line-number">4</span><br><span class="line-number">5</span><br><span class="line-number">6</span><br><span class="line-number">7</span><br><span class="line-number">8</span><br><span class="line-number">9</span><br><span class="line-number">10</span><br><span class="line-number">11</span><br><span class="line-number">12</span><br><span class="line-number">13</span><br><span class="line-number">14</span><br><span class="line-number">15</span><br><span class="line-number">16</span><br><span class="line-number">17</span><br><span class="line-number">18</span><br><span class="line-number">19</span><br><span class="line-number">20</span><br><span class="line-number">21</span><br><span class="line-number">22</span><br><span class="line-number">23</span><br><span class="line-number">24</span><br><span class="line-number">25</span><br><span class="line-number">26</span><br><span class="line-number">27</span><br><span class="line-number">28</span><br><span class="line-number">29</span><br><span class="line-number">30</span><br><span class="line-number">31</span><br><span class="line-number">32</span><br></div></div><p><code>appsettings.json</code>:</p><div class="language-json vp-adaptive-theme line-numbers-mode"><button title="Copy Code" class="copy"></button><span class="lang">json</span><pre class="shiki shiki-themes one-dark-pro one-light vp-code" tabindex="0"><code><span class="line"><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">{</span></span>
<span class="line"><span style="--shiki-dark:#E06C75;--shiki-light:#E45649;">  &quot;CensusQueryOptions&quot;</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">: {</span></span>
<span class="line"><span style="--shiki-dark:#E06C75;--shiki-light:#E45649;">    &quot;ServiceId&quot;</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">: </span><span style="--shiki-dark:#98C379;--shiki-light:#50A14F;">&quot;example&quot;</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">,</span></span>
<span class="line"><span style="--shiki-dark:#E06C75;--shiki-light:#E45649;">    &quot;LanguageCode&quot;</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">: </span><span style="--shiki-dark:#98C379;--shiki-light:#50A14F;">&quot;en&quot;</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">,</span></span>
<span class="line"><span style="--shiki-dark:#E06C75;--shiki-light:#E45649;">    &quot;Limit&quot;</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">: </span><span style="--shiki-dark:#D19A66;--shiki-light:#986801;">10</span></span>
<span class="line"><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">  }</span></span>
<span class="line"><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">}</span></span></code></pre><div class="line-numbers-wrapper" aria-hidden="true"><span class="line-number">1</span><br><span class="line-number">2</span><br><span class="line-number">3</span><br><span class="line-number">4</span><br><span class="line-number">5</span><br><span class="line-number">6</span><br><span class="line-number">7</span><br></div></div>`,9)]))}const B=i(l,[["render",t]]);export{o as __pageData,B as default};
