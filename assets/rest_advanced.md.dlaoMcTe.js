import{_ as e,c as a,o as i,ae as n}from"./chunks/framework.DHm_ITtO.js";const k=JSON.parse('{"title":"Advanced Query Setup","description":"","frontmatter":{},"headers":[],"relativePath":"rest/advanced.md","filePath":"rest/advanced.md"}'),t={name:"rest/advanced.md"};function r(l,s,p,h,d,o){return i(),a("div",null,s[0]||(s[0]=[n(`<h1 id="advanced-query-setup" tabindex="-1">Advanced Query Setup <a class="header-anchor" href="#advanced-query-setup" aria-label="Permalink to “Advanced Query Setup”">​</a></h1><h2 id="customizing-polly" tabindex="-1">Customizing Polly <a class="header-anchor" href="#customizing-polly" aria-label="Permalink to “Customizing Polly”">​</a></h2><p>It was mentioned in <a href="./">Getting Started</a> that <a href="https://github.com/App-vNext/Polly#wait-and-retry" target="_blank" rel="noreferrer">wait-and-retry</a> Polly policy is configured by default. This waits a certain amount of time after a failed query and then re-attempts it. The number of times the query is retried can be configured when registering the Census REST services:</p><div class="language-csharp vp-adaptive-theme line-numbers-mode"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes one-dark-pro one-light vp-code" tabindex="0"><code><span class="line"><span style="--shiki-dark:#E5C07B;--shiki-light:#383A42;">services</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">.</span><span style="--shiki-dark:#61AFEF;--shiki-light:#4078F2;">AddCensusRestServices</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">(</span><span style="--shiki-dark:#E5C07B;--shiki-light:#383A42;">maxRetryAttempts</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">: </span><span style="--shiki-dark:#D19A66;--shiki-light:#986801;">5</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">);</span></span></code></pre><div class="line-numbers-wrapper" aria-hidden="true"><span class="line-number">1</span><br></div></div><h2 id="using-alternative-census-implementations" tabindex="-1">Using Alternative Census Implementations <a class="header-anchor" href="#using-alternative-census-implementations" aria-label="Permalink to “Using Alternative Census Implementations”">​</a></h2><p>With Census being, at one point in its history, two years out of date on any static data, the API developer community began to extract and share information through alternative means. Some of these efforts have included Census-like API implementations. We&#39;ll be using <a href="https://github.com/PS2Sanctuary/Sanctuary.Census" target="_blank" rel="noreferrer">Sanctuary.Census</a> as one such example.</p><p><code>DbgCensus</code> can be easily configured to use alternative Census implementations, provided their interface is similar. The <a href="https://github.com/carlst99/DbgCensus/tree/main/Samples/RestSample" target="_blank" rel="noreferrer">REST Sample</a> demonstrates the following instruction by means of configuring a <em>named options</em> instance, so that the configured third-party Census options can be injected anywhere.</p><ol><li><p>Create a <code>CensusQueryOptions</code> instance, and point it towards the alternative Census implementation:</p><div class="language-csharp vp-adaptive-theme line-numbers-mode"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes one-dark-pro one-light vp-code" tabindex="0"><code><span class="line"><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;">CensusQueryOptions</span><span style="--shiki-dark:#E06C75;--shiki-light:#383A42;"> sanctuaryQueryOptions</span><span style="--shiki-dark:#56B6C2;--shiki-light:#383A42;"> =</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;"> new()</span></span>
<span class="line"><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">{</span></span>
<span class="line"><span style="--shiki-dark:#E06C75;--shiki-light:#383A42;">    RootEndpoint</span><span style="--shiki-dark:#56B6C2;--shiki-light:#383A42;"> =</span><span style="--shiki-dark:#98C379;--shiki-light:#50A14F;"> &quot;https://census.lithafalcon.cc&quot;</span></span>
<span class="line"><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">}</span></span></code></pre><div class="line-numbers-wrapper" aria-hidden="true"><span class="line-number">1</span><br><span class="line-number">2</span><br><span class="line-number">3</span><br><span class="line-number">4</span><br></div></div></li><li><p>Then, simply pass in these options when creating a query builder.</p><div class="language-csharp vp-adaptive-theme line-numbers-mode"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes one-dark-pro one-light vp-code" tabindex="0"><code><span class="line"><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;">QueryBuilder</span><span style="--shiki-dark:#E06C75;--shiki-light:#383A42;"> builder</span><span style="--shiki-dark:#56B6C2;--shiki-light:#383A42;"> =</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;"> new(</span><span style="--shiki-dark:#E06C75;--shiki-light:#383A42;">sanctuaryQueryOptions</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">);</span></span>
<span class="line"><span style="--shiki-dark:#7F848E;--shiki-dark-font-style:italic;--shiki-light:#A0A1A7;--shiki-light-font-style:italic;">// OR</span></span>
<span class="line"><span style="--shiki-dark:#E5C07B;--shiki-light:#C18401;">IQueryBuilder</span><span style="--shiki-dark:#E06C75;--shiki-light:#383A42;"> builder</span><span style="--shiki-dark:#56B6C2;--shiki-light:#383A42;"> =</span><span style="--shiki-dark:#E5C07B;--shiki-light:#383A42;"> _queryService</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">.</span><span style="--shiki-dark:#61AFEF;--shiki-light:#4078F2;">CreateQuery</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">(</span><span style="--shiki-dark:#E06C75;--shiki-light:#383A42;">sanctuaryQueryOptions</span><span style="--shiki-dark:#ABB2BF;--shiki-light:#383A42;">);</span></span></code></pre><div class="line-numbers-wrapper" aria-hidden="true"><span class="line-number">1</span><br><span class="line-number">2</span><br><span class="line-number">3</span><br></div></div></li></ol>`,8)]))}const u=e(t,[["render",r]]);export{k as __pageData,u as default};
