function integrated_path_planning_comparison
    % %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    % %%  PATH PLANNING: SAFETY VS EFFICIENCY COMPARISON
    % %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

    % --- 1. Define Map Data ---
    start_pt = [5, 5];
    end_pt   = [45, 40];
    obstacles = [10, 15; 30, 25]; % [x, y]

    % --- 2. Create the App Layout ---
    fig = uifigure('Name', 'XAI Path Comparison Dashboard', ...
                   'Position', [100, 100, 1000, 550]);

    g = uigridlayout(fig, [1, 2]);
    g.ColumnWidth = {'2x', '1x'};

    % -- Left Panel: The Plot --
    ax = uiaxes(g);
    ax.Title.String = 'Trajectory Comparison';
    ax.XLabel.String = 'X Coordinate';
    ax.YLabel.String = 'Y Coordinate';
    ax.Layout.Column = 1;
    grid(ax, 'on');
    axis(ax, 'equal');
    xlim(ax, [0, 50]); ylim(ax, [0, 45]);

    % -- Right Panel: The Dashboard --
    dashGrid = uigridlayout(g, [3, 1]);
    dashGrid.Layout.Column = 2;
    dashGrid.RowHeight = {'fit', '1x', 'fit'};

    uilabel(dashGrid, 'Text', 'XAI Decision Log:', 'FontWeight', 'bold');
    logArea = uitextarea(dashGrid, 'Editable', 'off');
    
    runBtn = uibutton(dashGrid, 'Text', 'COMPARE PATHS', ...
                      'BackgroundColor', [0.3, 0.6, 1], ...
                      'FontWeight', 'bold', 'FontColor', 'white');
    
    % --- 3. The Simulation Logic ---
    runBtn.ButtonPushedFcn = @(btn, event) runSimulation();

    function runSimulation()
        cla(ax); logArea.Value = cell(0,1); disableControl(runBtn);
        
        % -- Setup Map --
        updateLog('SYSTEM: Scanning environment...');
        hold(ax, 'on');
        plot(ax, start_pt(1), start_pt(2), 'go', 'MarkerSize', 10, 'MarkerFaceColor', 'g', 'DisplayName', 'Start');
        plot(ax, end_pt(1), end_pt(2), 'rx', 'MarkerSize', 10, 'LineWidth', 2, 'DisplayName', 'Target');
        plot(ax, obstacles(:,1), obstacles(:,2), 'ks', 'MarkerSize', 12, 'MarkerFaceColor', 'r', 'DisplayName', 'Obstacles');
        legend(ax, 'Location', 'northwest');
        pause(0.5);

        % -- 1. Original Blocked Path --
        updateLog('ANALYSIS: Direct path is obstructed.');
        plot(ax, [start_pt(1), end_pt(1)], [start_pt(2), end_pt(2)], 'b--', 'DisplayName', 'Blocked');
        pause(0.5);

        % -- 2. Calculate "Safe" Path (Wide Berth) --
        % These are the waypoints from the "Safe" version
        safe_wp1 = [15, 10]; 
        safe_wp2 = [35, 20];
        
        % Calculate Total Distance (Efficiency Metric)
        dist_safe = getDist(start_pt, safe_wp1) + getDist(safe_wp1, safe_wp2) + getDist(safe_wp2, end_pt);
        
        updateLog('MODE A: Generating High-Safety Route...');
        updateLog(sprintf('   >> Buffer: Large (5u) | Est. Dist: %.1f units', dist_safe));
        
        % Draw Cyan Line
        plot(ax, [start_pt(1), safe_wp1(1)], [start_pt(2), safe_wp1(2)], 'c--', 'LineWidth', 2, 'DisplayName', 'Safe Path');
        plot(ax, [safe_wp1(1), safe_wp2(1)], [safe_wp1(2), safe_wp2(2)], 'c--', 'LineWidth', 2, 'HandleVisibility', 'off');
        plot(ax, [safe_wp2(1), end_pt(1)], [safe_wp2(2), end_pt(2)], 'c--', 'LineWidth', 2, 'HandleVisibility', 'off');
        pause(1.5);

        % -- 3. Calculate "Efficient" Path (Tight Berth) --
        % These are the waypoints from the "Efficient" version
        eff_wp1 = [12, 12];
        eff_wp2 = [33, 22];
        
        % Calculate Total Distance
        dist_eff = getDist(start_pt, eff_wp1) + getDist(eff_wp1, eff_wp2) + getDist(eff_wp2, end_pt);
        
        updateLog('MODE B: Optimizing for Efficiency...');
        updateLog(sprintf('   >> Buffer: Minimal (2u) | Est. Dist: %.1f units', dist_eff));

        % Draw Green Line (Animate this one)
        animatePath(start_pt, eff_wp1, 'g-', 3, 'Efficient Path');
        pause(0.5);
        animatePath(eff_wp1, eff_wp2, 'g-', 3, '');
        pause(0.5);
        animatePath(eff_wp2, end_pt, 'g-', 3, '');
        
        % -- 4. Final Comparison --
        saved = dist_safe - dist_eff;
        percent = (saved / dist_safe) * 100;
        updateLog('--------------------------------');
        updateLog('DECISION MATRIX COMPLETE:');
        updateLog(sprintf('Selected Mode B (Efficiency).'));
        updateLog(sprintf('Energy Saved: %.1f%% (%.1f units)', percent, saved));
        
        runBtn.Text = 'RESET';
        runBtn.Enable = 'on';
    end

    % -- Helper Functions --
    function d = getDist(p1, p2)
        d = sqrt((p2(1)-p1(1))^2 + (p2(2)-p1(2))^2);
    end

    function animatePath(p1, p2, style, width, name)
        if isempty(name)
            plot(ax, [p1(1), p2(1)], [p1(2), p2(2)], style, 'LineWidth', width, 'HandleVisibility', 'off');
        else
            plot(ax, [p1(1), p2(1)], [p1(2), p2(2)], style, 'LineWidth', width, 'DisplayName', name);
        end
    end

    function updateLog(msg)
        timestamp = datestr(now, 'HH:MM:SS');
        newEntry = sprintf('[%s] %s', timestamp, msg);
        currentLog = logArea.Value;
        if isempty(currentLog)
             logArea.Value = {newEntry};
        else
             logArea.Value = [currentLog; {newEntry}];
        end
        scroll(logArea, 'bottom');
    end

    function disableControl(btn)
        btn.Enable = 'off';
        btn.Text = 'COMPUTING...';
    end
end