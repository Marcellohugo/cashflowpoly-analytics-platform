// Fungsi file: Menggambar chart metric pada halaman detail pemain dari payload data-chart.
(() => {
            const chartNodes = Array.from(document.querySelectorAll(".js-metric-line-chart[data-chart]"));
            if (!chartNodes.length) {
                return;
            }

            const ns = "http://www.w3.org/2000/svg";
            const seriesPalette = [
                "#1ba784",
                "#2d7dd2",
                "#f4a84a",
                "#ef4e4e",
                "#6a5acd",
                "#00a6a6",
                "#b56576",
                "#5f8f00"
            ];
            const variablePalette = [
                "#1ba784",
                "#2d7dd2",
                "#f4a84a",
                "#ef4e4e",
                "#6a5acd",
                "#00a6a6",
                "#ff7f50",
                "#7f5af0",
                "#16a34a",
                "#d97706",
                "#0284c7",
                "#be123c"
            ];
            const colorForSeries = (index) => seriesPalette[index % seriesPalette.length];
            const colorForVariable = (index) => variablePalette[index % variablePalette.length];
            const runtimeConfig = window.cashflowpolyPlayerDetailCharts || {};
            const tooltipText = Object.assign({
                metric: "Metric",
                series: "Series",
                points: "Points",
                formula: "Formula",
                defaultFormula: "Source calculation not available",
                selectedDetail: "Selected detail",
                tapHint: "Tap a point or bar to inspect details.",
                itemPrefix: "Item"
            }, runtimeConfig.tooltipText || {});
            const chartStatusText = Object.assign({
                noSeries: "No chart series available.",
                noPoints: "No numeric points available.",
                invalidPayload: "Invalid chart payload."
            }, runtimeConfig.chartStatusText || {});
            const formatMetricValue = (value) => {
                if (typeof value !== "number" || !Number.isFinite(value)) {
                    return "-";
                }

                const abs = Math.abs(value);
                const decimals = abs >= 100 ? 0 : abs >= 10 ? 1 : 2;
                return value.toLocaleString(undefined, {
                    minimumFractionDigits: 0,
                    maximumFractionDigits: decimals
                });
            };
            const makeHtmlNode = (name, className = "") => {
                const node = document.createElement(name);
                if (className.length > 0) {
                    node.className = className;
                }
                return node;
            };
            const ensureBarInsightPanel = (hostCard, detailLabel) => {
                if (!hostCard) {
                    return null;
                }

                const resolvedDetailLabel =
                    typeof detailLabel === "string" && detailLabel.trim().length > 0
                        ? detailLabel.trim()
                        : tooltipText.formula;

                let panel = hostCard.querySelector(".js-chart-bar-insight");
                if (!panel) {
                    panel = makeHtmlNode("div", "chart-bar-insight js-chart-bar-insight");
                    panel.hidden = true;
                    panel.setAttribute("aria-live", "polite");

                    const title = makeHtmlNode("p", "chart-bar-insight-title");
                    title.textContent = tooltipText.selectedDetail;
                    panel.appendChild(title);

                    const hint = makeHtmlNode("p", "chart-bar-insight-hint");
                    hint.textContent = tooltipText.tapHint;
                    panel.appendChild(hint);

                    const makeRow = (label, keyClass, valueClass) => {
                        const row = makeHtmlNode("div", "chart-bar-insight-row");
                        const keyClassName = keyClass && keyClass.length > 0
                            ? `chart-bar-insight-key ${keyClass}`
                            : "chart-bar-insight-key";
                        const key = makeHtmlNode("span", keyClassName);
                        key.textContent = label;
                        const value = makeHtmlNode("span", `chart-bar-insight-value ${valueClass}`);
                        value.textContent = "-";
                        row.appendChild(key);
                        row.appendChild(value);
                        return row;
                    };

                    panel.appendChild(makeRow(
                        tooltipText.metric,
                        "js-chart-bar-insight-metric-key",
                        "js-chart-bar-insight-metric"));
                    panel.appendChild(makeRow(
                        tooltipText.points,
                        "js-chart-bar-insight-points-key",
                        "js-chart-bar-insight-points"));
                    panel.appendChild(makeRow(
                        resolvedDetailLabel,
                        "js-chart-bar-insight-detail-key",
                        "js-chart-bar-insight-formula"));

                    hostCard.appendChild(panel);
                }

                const detailKey = panel.querySelector(".js-chart-bar-insight-detail-key");
                if (detailKey) {
                    detailKey.textContent = resolvedDetailLabel;
                }

                return {
                    panel,
                    metric: panel.querySelector(".js-chart-bar-insight-metric"),
                    points: panel.querySelector(".js-chart-bar-insight-points"),
                    formula: panel.querySelector(".js-chart-bar-insight-formula"),
                    detailKey
                };
            };
            const revealBarInsight = (insight, detail) => {
                if (!insight) {
                    return;
                }

                insight.panel.hidden = false;
                if (insight.metric) {
                    insight.metric.textContent = detail.metric;
                }
                if (insight.points) {
                    insight.points.textContent = detail.points;
                }
                if (insight.formula) {
                    insight.formula.textContent = detail.formula;
                }
            };

            const makeNode = (name, attrs = {}) => {
                const node = document.createElementNS(ns, name);
                Object.entries(attrs).forEach(([key, value]) => {
                    node.setAttribute(key, String(value));
                });
                return node;
            };

            const clearNode = (node) => {
                while (node.firstChild) {
                    node.removeChild(node.firstChild);
                }
            };

            const drawEmpty = (svg, message) => {
                clearNode(svg);
                const vb = svg.viewBox && svg.viewBox.baseVal ? svg.viewBox.baseVal : null;
                const width = vb && vb.width ? vb.width : 840;
                const height = vb && vb.height ? vb.height : 360;
                const text = makeNode("text", {
                    x: width / 2,
                    y: height / 2,
                    "text-anchor": "middle",
                    "dominant-baseline": "middle",
                    fill: "#4f6e83",
                    "font-size": "13",
                    "font-family": "Nunito, Segoe UI, sans-serif"
                });
                text.textContent = message;
                svg.appendChild(text);
            };

            const buildPath = (points) => {
                if (!points.length) {
                    return "";
                }

                let path = `M ${points[0].x} ${points[0].y}`;
                for (let index = 1; index < points.length; index += 1) {
                    path += ` L ${points[index].x} ${points[index].y}`;
                }
                return path;
            };

            const normalizeLabel = (value, index) => {
                const text = String(value ?? "")
                    .replace(/[_-]+/g, " ")
                    .replace(/\s+/g, " ")
                    .trim();

                if (text.length > 0) {
                    return text;
                }

                return `${tooltipText.itemPrefix} ${index + 1}`;
            };

            const makeUniqueLabels = (labels) => {
                const usageMap = new Map();
                return labels.map((label) => {
                    const key = String(label ?? "").toLowerCase();
                    const nextCount = (usageMap.get(key) || 0) + 1;
                    usageMap.set(key, nextCount);
                    if (nextCount === 1) {
                        return label;
                    }

                    return `${label} (${nextCount})`;
                });
            };

            const wrapAxisLabel = (label, maxCharsPerLine = 16, maxLines = 2) => {
                const compactLabel = String(label ?? "").replace(/\s+/g, " ").trim();
                if (!compactLabel.length) {
                    return ["-"];
                }

                const words = compactLabel.split(" ");
                const lines = [];
                let currentLine = "";

                words.forEach((word) => {
                    const chunks = word.length > maxCharsPerLine
                        ? word.match(new RegExp(`.{1,${maxCharsPerLine}}`, "g")) || [word]
                        : [word];

                    chunks.forEach((chunk) => {
                        const candidate = currentLine.length > 0
                            ? `${currentLine} ${chunk}`
                            : chunk;

                        if (candidate.length <= maxCharsPerLine) {
                            currentLine = candidate;
                            return;
                        }

                        if (currentLine.length > 0) {
                            lines.push(currentLine);
                        }
                        currentLine = chunk;
                    });
                });

                if (currentLine.length > 0) {
                    lines.push(currentLine);
                }

                if (lines.length <= maxLines) {
                    return lines;
                }

                const trimmedLines = lines.slice(0, maxLines);
                const lastIndex = maxLines - 1;
                const lastLine = trimmedLines[lastIndex];
                if (lastLine.length >= maxCharsPerLine - 1) {
                    trimmedLines[lastIndex] = `${lastLine.slice(0, Math.max(1, maxCharsPerLine - 1)).trim()}...`;
                } else {
                    trimmedLines[lastIndex] = `${lastLine}...`;
                }

                return trimmedLines;
            };

            const drawChart = (svg, payload) => {
                const normalizedLabels = Array.isArray(payload?.labels)
                    ? payload.labels.map((value, index) => normalizeLabel(value, index))
                    : [];
                const labels = makeUniqueLabels(normalizedLabels);
                const chartType = String(payload?.chartType ?? "line").toLowerCase();
                const metricKeys = Array.isArray(payload?.keys)
                    ? payload.keys.map((value) => String(value ?? ""))
                    : [];
                const formulaHints = Array.isArray(payload?.formulas)
                    ? payload.formulas.map((value) => String(value ?? "").trim())
                    : [];
                const detailLabel = typeof payload?.detailLabel === "string" && payload.detailLabel.trim().length > 0
                    ? payload.detailLabel.trim()
                    : tooltipText.formula;
                const detailFallback = typeof payload?.detailFallback === "string" && payload.detailFallback.trim().length > 0
                    ? payload.detailFallback.trim()
                    : tooltipText.defaultFormula;
                const series = Array.isArray(payload?.series)
                    ? payload.series
                        .map((item, index) => {
                            const values = Array.isArray(item?.values)
                                ? item.values.map((value) => {
                                    if (typeof value !== "number" || !Number.isFinite(value)) {
                                        return null;
                                    }
                                    return value;
                                })
                                : [];

                            return {
                                name: String(item?.name ?? `${tooltipText.series} ${index + 1}`),
                                values
                            };
                        })
                        .filter((item) => item.values.some((value) => value !== null))
                    : [];

                if (!labels.length || !series.length) {
                    drawEmpty(svg, chartStatusText.noSeries);
                    return;
                }

                const numericValues = [];
                series.forEach((item) => {
                    item.values.forEach((value) => {
                        if (value !== null) {
                            numericValues.push(value);
                        }
                    });
                });

                if (!numericValues.length) {
                    drawEmpty(svg, chartStatusText.noPoints);
                    return;
                }

                clearNode(svg);

                const vb = svg.viewBox && svg.viewBox.baseVal ? svg.viewBox.baseVal : null;
                const baseWidth = vb && vb.width ? vb.width : 840;
                const height = vb && vb.height ? vb.height : 360;
                const longestLabelLength = labels.reduce((max, label) => Math.max(max, label.length), 0);
                const averageLabelLength = labels.length
                    ? labels.reduce((total, label) => total + label.length, 0) / labels.length
                    : 0;
                const hostCard = svg.closest(".chart-card");
                const chartInsight = ensureBarInsightPanel(hostCard, detailLabel);
                if (chartInsight) {
                    chartInsight.panel.hidden = true;
                }
                let width = baseWidth;
                let barLabelCharsPerLine = 16;
                let wrappedAxisLabels = [];
                if (chartType === "bar") {
                    const minPitchPerLabel = Math.min(260, Math.max(
                        124,
                        longestLabelLength * 7.1,
                        averageLabelLength * 6.6 + 42));
                    const dynamicWidth = Math.max(baseWidth, labels.length * minPitchPerLabel + 280);
                    width = dynamicWidth;
                    barLabelCharsPerLine = Math.max(11, Math.min(22, Math.round(minPitchPerLabel / 8)));
                    wrappedAxisLabels = labels.map((label) => wrapAxisLabel(label, barLabelCharsPerLine, 2));
                    svg.style.width = `${dynamicWidth}px`;
                    svg.style.maxWidth = "none";
                    hostCard?.classList.add("chart-card-hscroll");
                } else {
                    svg.style.width = "100%";
                    svg.style.maxWidth = "100%";
                    hostCard?.classList.remove("chart-card-hscroll");
                }
                svg.setAttribute("viewBox", `0 0 ${width} ${height}`);

                const legendLineHeight = 16;
                const estimateLegendRows = () => {
                    if (chartType === "bar" && series.length === 1) {
                        return 0;
                    }

                    const rowMaxWidth = Math.max(180, width - 110);
                    let rowWidth = 0;
                    let rows = 1;
                    series.forEach((item) => {
                        const itemWidth = Math.max(110, item.name.length * 8 + 36);
                        if (rowWidth + itemWidth > rowMaxWidth) {
                            rows += 1;
                            rowWidth = itemWidth;
                            return;
                        }

                        rowWidth += itemWidth;
                    });

                    return rows;
                };

                const legendRows = estimateLegendRows();
                const margin = {
                    top: 16 + legendRows * legendLineHeight + 12,
                    right: chartType === "bar" ? 52 : 24,
                    bottom: chartType === "bar"
                        ? 80 + Math.max(0, (
                            wrappedAxisLabels.reduce((max, lines) => Math.max(max, lines.length), 1) - 1
                        ) * 13)
                        : 58,
                    left: 62
                };
                const plotWidth = Math.max(80, width - margin.left - margin.right);
                const plotHeight = Math.max(80, height - margin.top - margin.bottom);

                let minY = Math.min(...numericValues);
                let maxY = Math.max(...numericValues);
                const hasNegativeValues = numericValues.some((value) => value < 0);
                if (chartType === "bar") {
                    minY = Math.min(minY, 0);
                    maxY = Math.max(maxY, 0);
                    const range = Math.max(0.000001, maxY - minY);
                    const topPad = Math.max(0.5, range * 0.09);
                    maxY += topPad;
                    if (hasNegativeValues) {
                        minY -= topPad * 0.35;
                    }
                }
                if (Math.abs(maxY - minY) < 0.000001) {
                    const pad = Math.abs(maxY) < 1 ? 1 : Math.abs(maxY) * 0.1;
                    minY -= pad;
                    maxY += pad;
                }

                const clusterWidth = plotWidth / Math.max(1, labels.length);
                const xForIndex = (index) => {
                    if (chartType === "bar") {
                        return margin.left + clusterWidth * index + clusterWidth / 2;
                    }

                    if (labels.length === 1) {
                        return margin.left + plotWidth / 2;
                    }

                    return margin.left + (index * plotWidth) / (labels.length - 1);
                };

                const yForValue = (value) => (
                    margin.top + ((maxY - value) * plotHeight) / (maxY - minY)
                );
                const zeroY = yForValue(0);

                const gridTicks = 4;
                for (let tick = 0; tick <= gridTicks; tick += 1) {
                    const ratio = tick / gridTicks;
                    const y = margin.top + ratio * plotHeight;
                    const value = maxY - (maxY - minY) * ratio;

                    svg.appendChild(makeNode("line", {
                        x1: margin.left,
                        y1: y,
                        x2: margin.left + plotWidth,
                        y2: y,
                        stroke: "rgba(60, 125, 145, 0.18)",
                        "stroke-width": "1"
                    }));

                    const label = makeNode("text", {
                        x: margin.left - 8,
                        y: y + 4,
                        "text-anchor": "end",
                        fill: "#5f7f92",
                        "font-size": "12",
                        "font-family": "Nunito, Segoe UI, sans-serif"
                    });
                    label.textContent = value.toFixed(1).replace(/\.0$/, "");
                    svg.appendChild(label);
                }

                svg.appendChild(makeNode("line", {
                    x1: margin.left,
                    y1: margin.top,
                    x2: margin.left,
                    y2: margin.top + plotHeight,
                    stroke: "#9ccfd2",
                    "stroke-width": "1.4"
                }));

                svg.appendChild(makeNode("line", {
                    x1: margin.left,
                    y1: chartType === "bar" ? zeroY : margin.top + plotHeight,
                    x2: margin.left + plotWidth,
                    y2: chartType === "bar" ? zeroY : margin.top + plotHeight,
                    stroke: "#9ccfd2",
                    "stroke-width": "1.4"
                }));

                const xLabelStep = chartType === "bar"
                    ? 1
                    : Math.max(1, Math.ceil(labels.length / 6));
                labels.forEach((label, index) => {
                    const isLast = index === labels.length - 1;
                    if (!isLast && index % xLabelStep !== 0) {
                        return;
                    }

                    if (chartType === "bar") {
                        const x = xForIndex(index);
                        const y = margin.top + plotHeight + 18;
                        const labelLines = wrappedAxisLabels[index] || [label];
                        const text = makeNode("text", {
                            x,
                            y,
                            "text-anchor": "middle",
                            fill: "#5f7f92",
                            "font-size": "10.5",
                            "font-family": "Nunito, Segoe UI, sans-serif"
                        });
                        labelLines.forEach((line, lineIndex) => {
                            const tspan = makeNode("tspan", {
                                x,
                                dy: lineIndex === 0 ? 0 : 12
                            });
                            tspan.textContent = line;
                            text.appendChild(tspan);
                        });
                        if (labelLines.join(" ").replace(/\.\.\.$/, "").trim() !== label) {
                            const fullLabel = makeNode("title");
                            fullLabel.textContent = label;
                            text.appendChild(fullLabel);
                        }
                        svg.appendChild(text);
                        return;
                    }

                    const text = makeNode("text", {
                        x: xForIndex(index),
                        y: margin.top + plotHeight + 18,
                        "text-anchor": "middle",
                        fill: "#5f7f92",
                        "font-size": "11",
                        "font-family": "Nunito, Segoe UI, sans-serif"
                    });
                    text.textContent = label;
                    svg.appendChild(text);
                });

                if (chartType === "bar") {
                    const seriesCount = Math.max(1, series.length);
                    const seriesGap = seriesCount > 1
                        ? Math.max(2, Math.min(8, clusterWidth * 0.05))
                        : 0;
                    const clusterInnerWidth = Math.max(12, Math.min(220, clusterWidth * 0.72));
                    const rawBarWidth =
                        (clusterInnerWidth - seriesGap * Math.max(0, seriesCount - 1)) / seriesCount;
                    const barWidth = Math.max(4, Math.min(56, rawBarWidth));
                    const barGroupWidth = barWidth * seriesCount + seriesGap * Math.max(0, seriesCount - 1);
                    const groupOffset = Math.max(0, (clusterWidth - barGroupWidth) / 2);

                    series.forEach((item, seriesIndex) => {
                        const seriesColor = colorForSeries(seriesIndex);
                        item.values.forEach((value, index) => {
                            if (value === null) {
                                return;
                            }

                            const x =
                                margin.left +
                                clusterWidth * index +
                                groupOffset +
                                seriesIndex * (barWidth + seriesGap);
                            const y = yForValue(value);
                            const barTop = Math.min(y, zeroY);
                            const barHeight = Math.max(1.5, Math.abs(zeroY - y));
                            const barColor = colorForVariable(index);
                            const rect = makeNode("rect", {
                                x,
                                y: barTop,
                                width: barWidth,
                                height: barHeight,
                                rx: "2",
                                fill: barColor,
                                stroke: series.length > 1 ? seriesColor : "none",
                                "stroke-width": series.length > 1 ? "1.2" : "0",
                                opacity: "0.92"
                            });
                            const variableLabel =
                                labels[index] ||
                                metricKeys[index] ||
                                `${tooltipText.itemPrefix} ${index + 1}`;
                            const tooltipLines = [
                                `${tooltipText.metric}: ${variableLabel}`,
                                ...(series.length > 1 ? [`${tooltipText.series}: ${item.name}`] : []),
                                `${tooltipText.points}: ${formatMetricValue(value)}`,
                                `${detailLabel}: ${formulaHints[index] || detailFallback}`
                            ];
                            const insightDetail = {
                                metric: variableLabel,
                                points: formatMetricValue(value),
                                formula: formulaHints[index] || detailFallback
                            };
                            const title = makeNode("title");
                            title.textContent = tooltipLines.join("\n");
                            rect.appendChild(title);
                            rect.setAttribute("tabindex", "0");
                            rect.setAttribute("role", "button");
                            rect.setAttribute("aria-label", tooltipLines.join(". "));
                            rect.style.cursor = "pointer";
                            const onSelectBar = () => revealBarInsight(chartInsight, insightDetail);
                            rect.addEventListener("click", (event) => {
                                event.preventDefault();
                                onSelectBar();
                            });
                            rect.addEventListener("keydown", (event) => {
                                if (event.key === "Enter" || event.key === " ") {
                                    event.preventDefault();
                                    onSelectBar();
                                }
                            });
                            svg.appendChild(rect);
                        });
                    });
                } else {
                    series.forEach((item, seriesIndex) => {
                        const color = colorForSeries(seriesIndex);
                        let segment = [];

                        const flushSegment = () => {
                            if (!segment.length) {
                                return;
                            }

                            const path = buildPath(segment);
                            if (path.length) {
                                svg.appendChild(makeNode("path", {
                                    d: path,
                                    fill: "none",
                                    stroke: color,
                                    "stroke-width": "2.4",
                                    "stroke-linejoin": "round",
                                    "stroke-linecap": "round"
                                }));
                            }
                            segment = [];
                        };

                        item.values.forEach((value, index) => {
                            if (value === null) {
                                flushSegment();
                                return;
                            }

                            const point = { x: xForIndex(index), y: yForValue(value) };
                            segment.push(point);
                            const pointLabel =
                                labels[index] ||
                                metricKeys[index] ||
                                `${tooltipText.itemPrefix} ${index + 1}`;
                            const metricLabel = series.length > 1
                                ? `${item.name} - ${pointLabel}`
                                : pointLabel;
                            const tooltipLines = [
                                `${tooltipText.metric}: ${metricLabel}`,
                                ...(series.length > 1 ? [`${tooltipText.series}: ${item.name}`] : []),
                                `${tooltipText.points}: ${formatMetricValue(value)}`,
                                `${detailLabel}: ${formulaHints[index] || detailFallback}`
                            ];
                            const insightDetail = {
                                metric: metricLabel,
                                points: formatMetricValue(value),
                                formula: formulaHints[index] || detailFallback
                            };
                            const hitTarget = makeNode("circle", {
                                cx: point.x,
                                cy: point.y,
                                r: "10",
                                fill: "transparent"
                            });
                            const pointNode = makeNode("circle", {
                                cx: point.x,
                                cy: point.y,
                                r: "3",
                                fill: color,
                                stroke: "#ffffff",
                                "stroke-width": "1"
                            });
                            const title = makeNode("title");
                            title.textContent = tooltipLines.join("\n");
                            pointNode.appendChild(title);
                            pointNode.setAttribute("tabindex", "0");
                            pointNode.setAttribute("role", "button");
                            pointNode.setAttribute("aria-label", tooltipLines.join(". "));
                            pointNode.style.cursor = "pointer";
                            hitTarget.style.cursor = "pointer";
                            const onSelectPoint = () => revealBarInsight(chartInsight, insightDetail);
                            hitTarget.addEventListener("click", (event) => {
                                event.preventDefault();
                                onSelectPoint();
                            });
                            pointNode.addEventListener("click", (event) => {
                                event.preventDefault();
                                onSelectPoint();
                            });
                            pointNode.addEventListener("keydown", (event) => {
                                if (event.key === "Enter" || event.key === " ") {
                                    event.preventDefault();
                                    onSelectPoint();
                                }
                            });
                            svg.appendChild(hitTarget);
                            svg.appendChild(pointNode);
                        });
                        flushSegment();
                    });
                }

                if (!(chartType === "bar" && series.length === 1)) {
                    const legendStartX = margin.left;
                    let legendX = legendStartX;
                    let legendRow = 0;
                    const legendYBase = 16;
                    const legendMaxX = margin.left + plotWidth;
                    series.forEach((item, index) => {
                        const color = colorForSeries(index);
                        const itemWidth = Math.max(110, item.name.length * 8 + 36);
                        if (legendX + itemWidth > legendMaxX) {
                            legendRow += 1;
                            legendX = legendStartX;
                        }

                        const legendY = legendYBase + legendRow * legendLineHeight;
                        if (chartType === "bar") {
                            svg.appendChild(makeNode("rect", {
                                x: legendX,
                                y: legendY - 4.5,
                                width: "12",
                                height: "9",
                                rx: "2",
                                fill: color
                            }));
                        } else {
                            svg.appendChild(makeNode("line", {
                                x1: legendX,
                                y1: legendY,
                                x2: legendX + 14,
                                y2: legendY,
                                stroke: color,
                                "stroke-width": "2.2",
                                "stroke-linecap": "round"
                            }));
                        }

                        const label = makeNode("text", {
                            x: legendX + 18,
                            y: legendY + 4,
                            fill: "#32576e",
                            "font-size": "11",
                            "font-family": "Nunito, Segoe UI, sans-serif"
                        });
                        label.textContent = item.name;
                        svg.appendChild(label);

                        legendX += itemWidth;
                    });
                }
            };

            chartNodes.forEach((node) => {
                try {
                    const payload = JSON.parse(node.dataset.chart || "{}");
                    drawChart(node, payload);
                } catch {
                    drawEmpty(node, chartStatusText.invalidPayload);
                }
            });
        })();

