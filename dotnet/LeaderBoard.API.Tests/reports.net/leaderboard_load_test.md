> test info



test suite: `API`

test name: `LeaderBoard Load Test`

session id: `2025-06-15_02.48.94_session_bf7ebcff`

> scenario stats



scenario: `update_score_scenario`

  - ok count: `3000`

  - fail count: `0`

  - all data: `0.4` MB

  - duration: `00:00:30`

load simulations:

  - `inject`, rate: `100`, interval: `00:00:01`, during: `00:00:30`

|step|ok stats|
|---|---|
|name|`global information`|
|request count|all = `3000`, ok = `3000`, RPS = `100`|
|latency (ms)|min = `0.2`, mean = `0.72`, max = `28.27`, StdDev = `0.71`|
|latency percentile (ms)|p50 = `0.67`, p75 = `0.76`, p95 = `1.13`, p99 = `1.92`|
|data transfer (KB)|min = `0.144`, mean = `0.149`, max = `0.151`, all = `0.4` MB|
|||
|name|`update_score`|
|request count|all = `3000`, ok = `3000`, RPS = `100`|
|latency (ms)|min = `0.2`, mean = `0.71`, max = `28.25`, StdDev = `0.7`|
|latency percentile (ms)|p50 = `0.66`, p75 = `0.75`, p95 = `1.11`, p99 = `1.9`|
|data transfer (KB)|min = `0.144`, mean = `0.149`, max = `0.151`, all = `0.4` MB|


> status codes for scenario: `update_score_scenario`



|status code|count|message|
|---|---|---|
|OK|3000||


> scenario stats



scenario: `get_leaderboard_scenario`

  - ok count: `6000`

  - fail count: `0`

  - all data: `12.5` MB

  - duration: `00:00:30`

load simulations:

  - `inject`, rate: `200`, interval: `00:00:01`, during: `00:00:30`

|step|ok stats|
|---|---|
|name|`global information`|
|request count|all = `6000`, ok = `6000`, RPS = `200`|
|latency (ms)|min = `0.22`, mean = `0.87`, max = `44.15`, StdDev = `2.49`|
|latency percentile (ms)|p50 = `0.68`, p75 = `0.78`, p95 = `1.16`, p99 = `2.4`|
|data transfer (KB)|min = `0.201`, mean = `2.128`, max = `4.193`, all = `12.5` MB|
|||
|name|`get_leaderboard`|
|request count|all = `6000`, ok = `6000`, RPS = `200`|
|latency (ms)|min = `0.22`, mean = `0.87`, max = `44.15`, StdDev = `2.49`|
|latency percentile (ms)|p50 = `0.68`, p75 = `0.77`, p95 = `1.15`, p99 = `2.37`|
|data transfer (KB)|min = `0.201`, mean = `2.128`, max = `4.193`, all = `12.5` MB|


> status codes for scenario: `get_leaderboard_scenario`



|status code|count|message|
|---|---|---|
|OK|6000||


> scenario stats



scenario: `get_customer_rank_scenario`

  - ok count: `57`

  - fail count: `2943`

  - all data: `0.1` MB

  - duration: `00:00:30`

load simulations:

  - `inject`, rate: `100`, interval: `00:00:01`, during: `00:00:30`

|step|ok stats|
|---|---|
|name|`global information`|
|request count|all = `3000`, ok = `57`, RPS = `1.9`|
|latency (ms)|min = `0.33`, mean = `1.43`, max = `43.58`, StdDev = `5.63`|
|latency percentile (ms)|p50 = `0.67`, p75 = `0.77`, p95 = `0.94`, p99 = `1.29`|
|data transfer (KB)|min = `0.269`, mean = `1.412`, max = `3.485`, all = `0.1` MB|
|||
|name|`get_customer_rank`|
|request count|all = `3000`, ok = `57`, RPS = `1.9`|
|latency (ms)|min = `0.33`, mean = `1.42`, max = `43.58`, StdDev = `5.63`|
|latency percentile (ms)|p50 = `0.66`, p75 = `0.76`, p95 = `0.93`, p99 = `1.26`|
|data transfer (KB)|min = `0.121`, mean = `0.286`, max = `0.454`, all = `0.0` MB|


|step|failures stats|
|---|---|
|name|`global information`|
|request count|all = `3000`, fail = `2943`, RPS = `98.1`|
|latency (ms)|min = `0.25`, mean = `0.92`, max = `43.86`, StdDev = `2.43`|
|latency percentile (ms)|p50 = `0.73`, p75 = `0.83`, p95 = `1.25`, p99 = `2.7`|
|||
|name|`get_customer_rank`|
|request count|all = `3000`, fail = `2943`, RPS = `98.1`|
|latency (ms)|min = `0.2`, mean = `0.82`, max = `43.74`, StdDev = `2.41`|
|latency percentile (ms)|p50 = `0.65`, p75 = `0.74`, p95 = `1.12`, p99 = `2.08`|
|data transfer (KB)|min = `0.104`, mean = `0.107`, max = `0.108`, all = `0.3` MB|


> status codes for scenario: `get_customer_rank_scenario`



|status code|count|message|
|---|---|---|
|OK|57||
|NotFound|2943||


