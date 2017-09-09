io.stdout:setvbuf("no") -- this is so that sublime will print things when they come (rather than buffering).


-- function Server:new(object)
-- 	object = object or {}
-- 	setmetatable(object, self)
-- 	self.__index = self
-- 	return object
-- end

require "ProtoGame"

game = ProtoGame:new()
local drawRect = false

function love.load(args)
	love.window.setMode(1920/2, 1080/2, {resizable = false, vsync = true, fullscreen = false})
	love.graphics.setDefaultFilter( 'nearest',  'nearest',  0 )
	-- love.window.setTitle("MultiFarm")
	love.math.setRandomSeed(os.time())
	math.randomseed(os.time())
	game:init(args)
	love.graphics.setBackgroundColor(30, 10, 60)
end

--[[
What I want to do is a simple stardew valley esq game with farming, so plants grow, and people can move around, and multiplayer.
Current goals:
-server hosts the world, even for singleplayer
consider doing more than lan worlds.
-client experiences it
- don't trust the client for anything.

-- start with inventory, sending level data to the player, player movement, and player placing/destroying things
-- we don't care about what they're holding, we only care about moving items around in the inventory.
-- when doing an action on the world, we just tell them what item we're using? and the server confirms that it's alright to use
]]--

function love.update(dt)
	game:update(dt)
	-- print(love.joystick.getJoystickCount())
	if drawRect or love.joystick.getJoystickCount() > 0 then
		love.graphics.setColor(255, 0, 0)
		love.graphics.rectangle("fill", 0, 0, 100, 100)
	end
end

function love.draw()
	game:draw()
end

function love.keypressed(key, unicode)
	game:keypressed(key, unicode)
	if key == "escape" then
		love.event.quit()
	end
end

function love.keyreleased(key, unicode)
	game:keyreleased(key, unicode)
end

function love.mousepressed(button, x, y)
	game:mousepressed(button, x, y)
end

function love.mousereleased(button, x, y)
	game:mousereleased(button, x, y)
end

function love.resize(w, h)
	game:resize(w, h)
end

function love.textinput(text)
	-- game:textinput(text)
end

function love.gamepadadded(gamepad)
	print("gamepad "..tostring(gamepad).." added")
	drawRect = true
end

function love.gamepadremoved(gamepad)
	print("gamepad "..tostring(gamepad).."removed")
	drawRect = false
end

function love.gamepadaxis(joystick, axis, value)
	drawRect = true
end