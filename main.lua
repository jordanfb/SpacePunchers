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

-- local rectx = 0
-- local recty = 0
-- local leftx = 0
-- local lefty = 0

function print(text)
	if #game.log > 20 then
		table.remove(game.log, 1)
	end
	table.insert(game.log, tostring(text))
end

function love.update(dt)
	game:update(dt)
	-- print(love.joystick.getJoystickCount())
	-- rectx = rectx + leftx*dt*100
	-- recty = recty + lefty*dt*100
end

function love.draw()
	game:draw()
	-- if drawRect then
	-- 	love.graphics.setColor(255, 0, 0)
	-- 	love.graphics.rectangle("fill", rectx, recty, 100, 100)
	-- end
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
	-- drawRect = true
	game:gamepadadded(gamepad)
end

function love.gamepadremoved(gamepad)
	print("gamepad "..tostring(gamepad).."removed")
	-- drawRect = false
	game:gamepadremoved(gamepad)
end

function love.gamepadaxis(joystick, axis, value)
	-- drawRect = true
	-- if axis == "leftx" then
	-- 	leftx = value
	-- elseif axis == "lefty" then
	-- 	lefty = value
	-- end
	game:gamepadaxis(joystick, axis, value)
end

function love.gamepadpressed(gamepad, button)
	game:gamepadpressed(gamepad, button)
end

function love.gamepadreleased(gamepad, button)
	game:gamepadreleased(gamepad, button)
end